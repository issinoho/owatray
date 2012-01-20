using System.Runtime.InteropServices;
using System.Text;

namespace DrunkenBakery.OWAtray.ShellIntegration
{
	internal class NativeWin32
	{
		public delegate int EnumWindowsProcDelegate(int hWnd, int lParam);

		public const int GwChild = 5;
		public const int GwHwndfirst = 0;
		public const int GwHwndlast = 1;
		public const int GwHwndnext = 2;
		public const int GwHwndprev = 3;
		public const int GwOwner = 4;
		public const int ScClose = 0xF060;
		public const int WmSyscommand = 0x0112;

		[DllImport("user32")]
		public static extern int EnumWindows(EnumWindowsProcDelegate lpEnumFunc, int lParam);

		[DllImport("user32.dll")]
		public static extern int FindWindow(
			string lpClassName, // class name
			string lpWindowName // window name
			);

		[DllImport("user32")]
		public static extern int GetDesktopWindow();

		[DllImport("user32")]
		public static extern int GetParent(int hwnd);

		[DllImport("user32")]
		public static extern int GetWindow(int hwnd, int wCmd);

		[DllImport("user32", EntryPoint = "GetWindowLongA")]
		public static extern int GetWindowLongPtr(int hwnd, int nIndex);

		[DllImport("User32.Dll")]
		public static extern void GetWindowText(int h, StringBuilder s, int nMaxCount);

		[DllImport("user32")]
		public static extern int IsWindowVisible(int hwnd);

		[DllImport("user32.dll")]
		public static extern int SendMessage(
			int hWnd, // handle to destination window
			uint msg, // message
			int wParam, // first message parameter
			int lParam // second message parameter
			);

		[DllImport("user32.dll")]
		public static extern int SetForegroundWindow(
			int hWnd // handle to window
			);
	}
}