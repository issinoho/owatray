#include <windows.h>
#include <tchar.h>
#include <mapi.h>
#include <iostream>
#include <string>
#include <fstream>
#include <direct.h>
#include <tchar.h>
using namespace std;

#define MAX_RECIPS  2000
#define MAX_FILES   100


#define           MAX_NAME_LEN    256
#define           MAX_PW_LEN      256
#define           MAX_MSGINFO_LEN 512
#define           MAX_POINTERS    32

const CLSID CLSID_CMapiImp = {0x29f458be, 0x8866, 0x11d5,
							  {0xa3, 0xdd, 0x0, 0xb0, 0xd0, 0xf3, 0xba, 0xa7}};

DWORD tId = 0;

// Used to make each MAPISendMail's temp attachment folder unique - see GetOwatrayLocalAppDataDir /
// MAPISendMail below.
static LONG g_sendCounter = 0;

#define   MAPI_MESSAGE_TYPE     0
#define   MAPI_RECIPIENT_TYPE   1

typedef struct {
  LPVOID    lpMem;
  UCHAR     memType;
} memTrackerType;


// this can't be right.
memTrackerType    memArray[MAX_POINTERS];

//
// For remembering memory...how ironic.
//
void
SetPointerArray(LPVOID ptr, BYTE type)
{
int i;

  for (i=0; i<MAX_POINTERS; i++)
  {
	if (memArray[i].lpMem == NULL)
	{
	  memArray[i].lpMem = ptr;
	  memArray[i].memType = type;
	  break;
	}
  }
}


BOOL WINAPI DllMain(HINSTANCE aInstance, DWORD aReason, LPVOID aReserved)
{
	switch (aReason)
	{
		case DLL_PROCESS_ATTACH : tId = TlsAlloc();
								  if (tId == 0xFFFFFFFF)
									  return FALSE;
								  break;

		case DLL_PROCESS_DETACH : TlsFree(tId);
								  break;
	}
	return TRUE;
}

////////////////////////////////////////////////////////////////////////////////////////
// Logging support. Everything lives under %LOCALAPPDATA%\OWAtray\ rather than a fixed  //
// drive path, since that's always writable by (and scoped to) the calling user - see   //
// MAPI.md for why the previous C:\temp\owamapi\ location was a problem.                //
////////////////////////////////////////////////////////////////////////////////////////

string gettimestring()
{
   SYSTEMTIME stime;
   GetLocalTime(&stime);
   char buf[40] = {0};
   sprintf_s(buf,"%02d%02d%04d%02d%02d%02d",
		stime.wDay, stime.wMonth, stime.wYear, stime.wHour, stime.wMinute, stime.wSecond);
   string dt = buf;
   return dt;
}

//
// Returns %LOCALAPPDATA%\OWAtray\<subfolder>, creating both it and the OWAtray folder above it if
// they don't exist yet. Returns an empty string if LOCALAPPDATA isn't set (callers treat that as
// "logging/temp storage unavailable" and skip it rather than fail the MAPI call).
//
string GetOwatrayLocalAppDataDir(const char *subfolder)
{
	char localAppData[MAX_PATH] = {0};
	if (!GetEnvironmentVariableA("LOCALAPPDATA", localAppData, MAX_PATH))
		return "";

	string root = string(localAppData) + "\\OWAtray";
	_mkdir(root.c_str());

	string dir = root + "\\" + subfolder;
	_mkdir(dir.c_str());

	return dir;
}

//
// Appends one timestamped line to %LOCALAPPDATA%\OWAtray\logs\debug.log. Every exported function
// logs through here instead of opening its own ofstream, so there's a single place that creates the
// directory and checks the file actually opened - previously only MAPISendMail created the
// directory first, so every other export's log line was silently dropped on a machine where
// MAPISendMail hadn't already run at least once.
//
void WriteLogLine(const string &message)
{
	string logDir = GetOwatrayLocalAppDataDir("logs");
	if (logDir.empty())
		return;

	ofstream File((logDir + "\\debug.log").c_str(), ios::app);
	if (!File.is_open())
		return;

	File << "\r\n" << gettimestring() << " " << message;
	File.close();
}

////////////////////////////////////////////////////////////////////////////////////////
// The MAPILogon function begins a Simple MAPI session, loading the default message ////
// store and address book providers                            ////
////////////////////////////////////////////////////////////////////////////////////////

ULONG FAR PASCAL MAPILogon(ULONG aUIParam, LPTSTR aProfileName,
							LPTSTR aPassword, FLAGS aFlags,
							ULONG aReserved, LPLHANDLE aSession)
{
	HRESULT hr = 0;
	ULONG nSessionId = 0;

	//if (!(aFlags & MAPI_UNICODE))
	//{
	//    // Need to convert the parameters to Unicode.

	//    char *pUserName = (char *) aProfileName;
	//    char *pPassWord = (char *) aPassword;

	//    TCHAR ProfileName[MAX_NAME_LEN] = {0};
	//    TCHAR PassWord[MAX_PW_LEN] = {0};

	//    if (pUserName != NULL)
	//    {
	//        if (!MultiByteToWideChar(CP_ACP, 0, pUserName, -1, ProfileName,
	//                                                        MAX_NAME_LEN))
	//            return MAPI_E_FAILURE;
	//    }

	//    if (pPassWord != NULL)
	//    {
	//        if (!MultiByteToWideChar(CP_ACP, 0, pPassWord, -1, PassWord,
	//                                                        MAX_NAME_LEN))
	//            return MAPI_E_FAILURE;
	//    }

	//}

	WriteLogLine("MAPILogon");

	int myHandle = 1;
	(*aSession) = (LHANDLE) myHandle;

	return SUCCESS_SUCCESS;
}



ULONG FAR PASCAL MAPILogoff (LHANDLE aSession, ULONG aUIParam,
											FLAGS aFlags, ULONG aReserved)
{
	WriteLogLine("MAPILogoff");

	return SUCCESS_SUCCESS;
}

std::string replaceOnce(
  std::string result,
  const std::string& replaceWhat,
  const std::string& replaceWithWhat)
{
  const int pos = result.find(replaceWhat);
  if (pos==-1) return result;
  result.replace(pos,replaceWhat.size(),replaceWithWhat);
  return result;
}

ULONG FAR PASCAL MAPISendMail (LHANDLE lhSession, ULONG ulUIParam, MapiMessage *lpMessage,
				FLAGS flFlags, ULONG ulReserved )
{
	unsigned long i;
	HRESULT hr = 0;
	BOOL bTempSession = FALSE ;

	WriteLogLine("MAPISendMail");

	if (lpMessage->nRecipCount > MAX_RECIPS)
		return MAPI_E_TOO_MANY_RECIPIENTS ;

	if (lpMessage->nFileCount > MAX_FILES)
		return MAPI_E_TOO_MANY_FILES ;

	if ( (!(flFlags & MAPI_DIALOG)) && (lpMessage->lpRecips == NULL) )
		return MAPI_E_UNKNOWN_RECIPIENT ;

	if (!lhSession)
	{
		FLAGS LoginFlag ;
		if ( (flFlags & MAPI_LOGON_UI) && (flFlags & MAPI_NEW_SESSION) )
			LoginFlag = MAPI_LOGON_UI | MAPI_NEW_SESSION ;
		else if (flFlags & MAPI_LOGON_UI)
			LoginFlag = MAPI_LOGON_UI ;

		hr = MAPILogon (ulUIParam, (LPTSTR) NULL, (LPTSTR) NULL, LoginFlag, 0, &lhSession) ;
		if (hr != SUCCESS_SUCCESS)
			return MAPI_E_LOGIN_FAILURE ;
		bTempSession = TRUE ;
	}

	// Unique temp folder for this send's attachments. A bare timestamp only has 1-second
	// resolution and two sends landing in the same second would collide on the same folder and mix
	// attachments together, so fold in the process ID and a per-process counter too.
	std::string tpath = "";
	string attachmentsDir = GetOwatrayLocalAppDataDir("mapi");
	if (!attachmentsDir.empty())
	{
		char suffix[64] = {0};
		sprintf_s(suffix, "%s-%lu-%lu", gettimestring().c_str(),
			(unsigned long) GetCurrentProcessId(), (unsigned long) InterlockedIncrement(&g_sendCounter));
		tpath = attachmentsDir + "\\" + suffix;
		_mkdir(tpath.c_str());
	}

	WriteLogLine("MAPISendMail-- Created temp folder (" + tpath + ")");

	// Get each file passed by MAPI
	for (i=0; i<lpMessage->nFileCount; i++)
	{
		lpMapiFileDesc attachment = lpMessage->lpFiles++;
		WriteLogLine("MAPISendMail-- (" + std::to_string(i + 1) + ") " + attachment->lpszFileName +
			" (" + attachment->lpszPathName + ")");

		// Copy to temp folder & rename
		string source = attachment->lpszPathName;
		TCHAR tsource[MAX_MSGINFO_LEN] = {0};
		MultiByteToWideChar(CP_ACP, 0, source.c_str(), -1, tsource, MAX_MSGINFO_LEN);
		string destination = tpath + "\\" + attachment->lpszFileName;
		TCHAR tdestination[MAX_MSGINFO_LEN] = {0};
		MultiByteToWideChar(CP_ACP, 0, destination.c_str(), -1, tdestination, MAX_MSGINFO_LEN);

		WriteLogLine("MAPISendMail-- Copying " + source + " to " + destination);
		CopyFile(tsource, tdestination, FALSE);
	}

	// Read registry to get name of handler
	string exePath;
	string parameters;
	LONG returnStatus;
	char lszValue[255];
	HKEY hKey;
	DWORD dwType=REG_SZ;
	DWORD dwSize=sizeof(lszValue);
	returnStatus = RegOpenKeyEx(HKEY_LOCAL_MACHINE, _T("SOFTWARE\\Clients\\Mail\\OWAMapi"), 0L,  KEY_QUERY_VALUE, &hKey);
	if (returnStatus == ERROR_SUCCESS)
	{
	  returnStatus = RegQueryValueExA(hKey, "EXE", NULL, &dwType,(BYTE *)lszValue, &dwSize);
	  if (returnStatus == ERROR_SUCCESS)
	  {
		  exePath = lszValue;
		  WriteLogLine("Value of HKLM\\SOFTWARE\\Clients\\Mail\\OWAMapi\\EXE is " + exePath);
	  }
	  returnStatus = RegQueryValueExA(hKey, "Parameters", NULL, &dwType,(BYTE *)lszValue, &dwSize);
	  if (returnStatus == ERROR_SUCCESS)
	  {
		  parameters = lszValue;
		  WriteLogLine("Value of HKLM\\SOFTWARE\\Clients\\Mail\\OWAMapi\\Parameters is " + parameters);
	  }
	  RegCloseKey(hKey);
	}
	else
	{
		WriteLogLine("MAPISendMail-- Could not open HKLM\\SOFTWARE\\Clients\\Mail\\OWAMapi (error " +
			std::to_string(returnStatus) + ") - is a mail handler registered?");
	}

	// Substitute temp folder path for %1 parameter
	parameters = replaceOnce(parameters, "%1", tpath);
	WriteLogLine("Replaced parameters are " + parameters);

	// Spawn EXE
	string cmdLine = exePath + " " + parameters;

	STARTUPINFO si = { sizeof(STARTUPINFO) };
	si.dwFlags = STARTF_USESHOWWINDOW;
	si.wShowWindow = SW_HIDE;
	PROCESS_INFORMATION pi;
	TCHAR tsource[MAX_MSGINFO_LEN] = {0};
	MultiByteToWideChar(CP_ACP, 0, cmdLine.c_str(), -1, tsource, MAX_MSGINFO_LEN);
	if (!CreateProcess(NULL, tsource, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi))
	{
		WriteLogLine("MAPISendMail-- CreateProcess failed (error " + std::to_string(GetLastError()) + ")");
	}
	else
	{
		CloseHandle(pi.hProcess);
		CloseHandle(pi.hThread);
	}

	if (bTempSession)
	MAPILogoff (lhSession, ulUIParam, 0,0) ;

	return hr ;
}

ULONG FAR PASCAL MAPISendDocuments(ULONG ulUIParam, LPTSTR lpszDelimChar, LPTSTR lpszFilePaths,
								LPTSTR lpszFileNames, ULONG ulReserved)
{
	LHANDLE lhSession ;

	unsigned long result = MAPILogon (ulUIParam, (LPTSTR) NULL, (LPTSTR) NULL, MAPI_LOGON_UI, 0, &lhSession) ;
	if (result != SUCCESS_SUCCESS)
		return MAPI_E_LOGIN_FAILURE ;

	MAPILogoff (lhSession, ulUIParam, 0,0) ;

	WriteLogLine("MAPISendDocuments");

	return SUCCESS_SUCCESS ;
}

ULONG FAR PASCAL MAPIFindNext(LHANDLE lhSession, ULONG ulUIParam, LPTSTR lpszMessageType,
							  LPTSTR lpszSeedMessageID, FLAGS flFlags, ULONG ulReserved,
							  unsigned char lpszMessageID[64])
{
  if (lhSession == 0)
	return(MAPI_E_INVALID_SESSION);

  if (!lpszMessageType)
	lpszMessageType = L"";

  if (!lpszSeedMessageID)
	lpszSeedMessageID = L"";

	WriteLogLine("MAPIFindNext");

  return SUCCESS_SUCCESS ;
}


ULONG FAR PASCAL MAPIReadMail(LHANDLE lhSession, ULONG ulUIParam, LPTSTR lpszMessageID,
							  FLAGS flFlags, ULONG ulReserved, lpMapiMessage **lppMessage)
{
  if (lhSession == 0)
	return(MAPI_E_INVALID_SESSION);

	WriteLogLine("MAPIReadMail");

  return SUCCESS_SUCCESS ;
}

ULONG FAR PASCAL MAPISaveMail(LHANDLE lhSession, ULONG ulUIParam, lpMapiMessage lpMessage,
							  FLAGS flFlags, ULONG ulReserved, LPTSTR lpszMessageID)
{
  if (lhSession == 0)
	return(MAPI_E_INVALID_SESSION);

	WriteLogLine("MAPISaveMail");

  return MAPI_E_FAILURE;
}

ULONG FAR PASCAL MAPIDeleteMail(LHANDLE lhSession, ULONG ulUIParam, LPTSTR lpszMessageID,
								FLAGS flFlags, ULONG ulReserved)
{
  if (lhSession == 0)
	return(MAPI_E_INVALID_SESSION);

	WriteLogLine("MAPIDeleteMail");

  return SUCCESS_SUCCESS ;
}

ULONG FAR PASCAL MAPIAddress(LHANDLE lhSession, ULONG ulUIParam, LPTSTR lpszCaption,
							 ULONG nEditFields, LPTSTR lpszLabels, ULONG nRecips,
							 lpMapiRecipDesc lpRecips, FLAGS flFlags,
							 ULONG ulReserved, LPULONG lpnNewRecips,
							 lpMapiRecipDesc FAR *lppNewRecips)
{
	return MAPI_E_FAILURE;
}

ULONG FAR PASCAL MAPIDetails(LHANDLE lhSession, ULONG ulUIParam, lpMapiRecipDesc lpRecip,
							 FLAGS flFlags, ULONG ulReserved)
{
	return MAPI_E_FAILURE;
}

ULONG FAR PASCAL MAPIResolveName(LHANDLE lhSession, ULONG ulUIParam, LPTSTR lpszName,
								 FLAGS flFlags, ULONG ulReserved, lpMapiRecipDesc FAR *lppRecip)
{
	return MAPI_E_FAILURE;
}

void FreeMAPIRecipient(lpMapiRecipDesc pv);
void FreeMAPIMessage(lpMapiMessage pv);

ULONG FAR PASCAL MAPIFreeBuffer(LPVOID pv)
{
  int   i;

  if (!pv)
	return(S_OK);

  for (i=0; i<MAX_POINTERS; i++)
  {
	if (pv == memArray[i].lpMem)
	{
	  if (memArray[i].memType == MAPI_MESSAGE_TYPE)
	  {
		FreeMAPIMessage((MapiMessage *)pv);
		memArray[i].lpMem = NULL;
	  }
	  else if (memArray[i].memType == MAPI_RECIPIENT_TYPE)
	  {
		FreeMAPIRecipient((MapiRecipDesc *)pv);
		memArray[i].lpMem = NULL;
	  }
	}
  }

  pv = NULL;
  return(S_OK);
}

ULONG FAR PASCAL GetMapiDllVersion()
{
	return 94;
}

void
FreeMAPIFile(lpMapiFileDesc pv)
{
  if (!pv)
	return;

  if (pv->lpszPathName != NULL)
	free(pv->lpszPathName);

  if (pv->lpszFileName != NULL)
	free(pv->lpszFileName);
}


void
FreeMAPIMessage(lpMapiMessage pv)
{
  ULONG i;

  if (!pv)
	return;

  if (pv->lpszSubject != NULL)
	free(pv->lpszSubject);

  if (pv->lpszNoteText)
	  free(pv->lpszNoteText);

  if (pv->lpszMessageType)
	free(pv->lpszMessageType);

  if (pv->lpszDateReceived)
	free(pv->lpszDateReceived);

  if (pv->lpszConversationID)
	free(pv->lpszConversationID);

  if (pv->lpOriginator)
	FreeMAPIRecipient(pv->lpOriginator);

  for (i=0; i<pv->nRecipCount; i++)
  {
	if (&(pv->lpRecips[i]) != NULL)
	{
	  FreeMAPIRecipient(&(pv->lpRecips[i]));
	}
  }

  if (pv->lpRecips != NULL)
  {
	free(pv->lpRecips);
  }

  for (i=0; i<pv->nFileCount; i++)
  {
	if (&(pv->lpFiles[i]) != NULL)
	{
	  FreeMAPIFile(&(pv->lpFiles[i]));
	}
  }

  if (pv->lpFiles != NULL)
  {
	free(pv->lpFiles);
  }

  free(pv);
  pv = NULL;
}

void
FreeMAPIRecipient(lpMapiRecipDesc pv)
{
  if (!pv)
	return;

  if (pv->lpszName != NULL)
	free(pv->lpszName);

  if (pv->lpszAddress != NULL)
	free(pv->lpszAddress);

  if (pv->lpEntryID != NULL)
	free(pv->lpEntryID);
}



