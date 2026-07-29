# The native Mapi DLL: what it actually does

`src/Mapi` (`MapiDll.vcxproj`, native C++) builds `DrunkenBakery.OWAtray.Mapi.dll` — the DLL registered
at `HKLM\SOFTWARE\Clients\Mail\OWAMapi\DLLPath` (see [`REGISTRY.md`](REGISTRY.md)). Any Windows
application that "sends via MAPI" (e.g. Word/Excel "Email" button, Explorer's "Send to → Mail
recipient") `LoadLibrary`s this DLL in-process and calls into it directly — it implements the classic
**Simple MAPI** C API (not Extended MAPI/Outlook Object Model), a 13-function contract from the early-90s
`mapi.h`. All 13 exports live in the single file `src/Mapi/MapiDll.cpp`; `src/Mapi/Mapi32.DEF` is the
module-definition file that lists which symbols get exported by name (Simple MAPI clients resolve them
via `GetProcAddress` by name, not ordinal).

`Mapi32.DEF` itself is not original to this project — its license header and `LIBRARY mozMapi32.dll`
line show it started life as Mozilla's own Simple MAPI support DLL (`mozMapi32.dll`, MPL 1.1/GPL
2.0/LGPL 2.1 tri-licensed). OWAtray's build overrides the actual output filename via
`<ProjectName>DrunkenBakery.OWAtray.Mapi</ProjectName>` in `MapiDll.vcxproj` (VC++ projects default
`TargetName` to `ProjectName`), so the `LIBRARY mozMapi32.dll` line in the `.DEF` is just an unused
leftover — the built DLL is really named `DrunkenBakery.OWAtray.Mapi.dll`, and nothing reads that
internal name at load time.

## What each exported function actually does

Most of the 13 exports are near-total stubs; only `MAPISendMail` and `MAPIFreeBuffer` do real work (and
`MAPIFreeBuffer`'s real work is unreachable — see below):

| Export | What it really does |
|---|---|
| `MAPILogon` | Doesn't authenticate anything. Fabricates a constant session handle (`1`) and always returns success, regardless of the profile name/password passed in. Appends `"MAPILogon"` to the debug log (see below). |
| `MAPILogoff` | Logs `"MAPILogoff"` and returns success. No session state to tear down, since `MAPILogon` never created any. |
| `MAPISendMail` | The one export with real behavior — see below. |
| `MAPISendDocuments` | Calls the `MAPILogon`/`MAPILogoff` stubs (so it always "succeeds") and logs `"MAPISendDocuments"`, but never does anything with the delimited file-path/file-name strings it's handed. Effectively a no-op. |
| `MAPIFindNext` | Validates the session handle is non-zero, logs, returns success. Never actually finds anything. |
| `MAPIReadMail` | Validates the session handle, logs, returns success — but never populates the `lppMessage` out-parameter, so a caller that takes this "success" at face value gets an uninitialized pointer. |
| `MAPISaveMail` | Validates the session handle, logs, then unconditionally returns `MAPI_E_FAILURE` — explicitly unimplemented. |
| `MAPIDeleteMail` | Validates the session handle, logs, returns success. Deletes nothing. |
| `MAPIAddress`, `MAPIDetails`, `MAPIResolveName` | Immediately return `MAPI_E_FAILURE`. No address-book support exists at all. |
| `MAPIFreeBuffer` | Real cleanup logic — walks a fixed 32-slot bookkeeping array (`memArray`) built by a `SetPointerArray` helper, and frees whichever of `MapiMessage`/`MapiRecipDesc` structures match the pointer being freed. **`SetPointerArray` is never called anywhere in this file**, so `memArray` is always empty and this function never actually frees anything it's handed — dead code in practice. |
| `GetMapiDllVersion` | Returns the constant `94`. `GetMapiDllVersion` isn't part of the standard Simple MAPI contract; it's a de facto extension some clients probe for. The value doesn't correspond to a documented version scheme — it's just whatever the original author set. |

## `MAPISendMail` — the actual bridge to the browser

This is the only export that does something meaningful with a message. It ignores the message content
entirely and forwards only the file attachments:

1. Rejects the call outright if `nRecipCount > 2000` or `nFileCount > 100` (`MAX_RECIPS`/`MAX_FILES`),
   or if there's no recipient list and the caller didn't pass `MAPI_DIALOG`.
2. If the caller didn't already have a session handle, opens one via the `MAPILogon` stub above (always
   succeeds) and remembers to log off again at the end.
3. Creates a fresh temp folder named for the current timestamp,
   `C:\temp\owamapi\<ddMMyyyyHHmmss>\`, and copies every attached file (`lpMessage->lpFiles`) into it
   under its original filename.
4. Reads `HKLM\SOFTWARE\Clients\Mail\OWAMapi\EXE` and `...\Parameters`
   (written by `ShellIntegration.exe`'s `InitRegistry` — see `REGISTRY.md`) to find the currently
   registered handler and its command-line template (`"<shell>" mapi %1`).
5. Substitutes the first `%1` in `Parameters` with the temp folder path from step 3.
6. Spawns that command line as a hidden child process (`CreateProcess`, `SW_HIDE`) — in practice this
   launches `DrunkenBakery.OWAtray.ShellIntegration.exe mapi <temp-folder>`, whose `DoMapi()` then reads
   every file back out of that same folder to attach them to the OWA compose window (see
   `src/ShellIntegration/Program.cs`).
7. Logs off the temp session opened in step 2, if any.

**What's silently dropped:** the message's subject, body (`lpszNoteText`), and recipient list
(`lpRecips`) are never read or passed anywhere — only file attachments survive the handoff from the
calling app to the browser compose window. A caller that does `MAPISendMail` with a subject/body and no
attachments gets a blank OWA compose window with nothing pre-filled.

## Logging and temp files

Every stub above (except `MAPIAddress`/`MAPIDetails`/`MAPIResolveName`/`MAPIFreeBuffer`/
`GetMapiDllVersion`) unconditionally opens `C:\temp\owamapi\debug.log` in append mode and writes one
line naming the function that was called. A few things worth knowing if you're debugging or touching
this code:

- The path is hardcoded to the `C:` drive — there's no per-user or configurable location, unlike the
  rest of the app (which installs to `$APPDATA` precisely to avoid needing a fixed drive/admin rights).
- Only `MAPISendMail` actually creates the `C:\temp\owamapi\` directory (`_mkdir`, twice, ignoring
  failure) before opening its log file / temp folder. Every other export tries to open
  `C:\temp\owamapi\debug.log` for append without first ensuring the directory exists; `ofstream` doesn't
  throw on failure to open, so on a machine where that directory was never created (e.g. `MAPISendMail`
  was never called first) those log lines are silently dropped rather than erroring.
- Nothing ever deletes the per-send timestamped temp folders created in `MAPISendMail` step 3, or the
  debug log itself — both accumulate indefinitely under `C:\temp\owamapi\`.

## Build

`MapiDll.vcxproj` is a native `DynamicLibrary` project built for both `Win32` and `x64` (the installer
ships both — see `.github/workflows/build.yml` and `Installer/OWAtray.nsi`). It targets the `v110`
(VS2012) platform toolset in the checked-in project file; CI overrides that to `v143` on the command line
since `windows-latest` doesn't have `v110` installed (see `CLAUDE.md`). `MapiDll.rc` carries the DLL's
version resource (`FILEVERSION`/`PRODUCTVERSION` `1,0,0,1`, unrelated to and never bumped alongside the
main app's `AssemblyVersion`/NSIS `PRODUCT_VERSION`).
