str se="$desktop$\tccwin2.exe"
__Tcc x.Compile("" se 1)

run se

#ret

//+---------------------------------------------------------------------------

#include <windows.h>

#define APPNAME "HELLO_WIN"

char szAppName[] = APPNAME; // The name of this application
char szTitle[]   = APPNAME; // The title bar text
char *pWindowText;

HINSTANCE g_hInst;          // current instance

void CenterWindow(HWND hWnd);

//+---------------------------------------------------------------------------

LRESULT CALLBACK WndProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
		// ----------------------- first and last
		case WM_CREATE:
			CenterWindow(hwnd);
			break;

		case WM_DESTROY:
			PostQuitMessage(0);
			break;


		// ----------------------- get out of it...
		case WM_RBUTTONUP:
			DestroyWindow(hwnd);
			break;

		case WM_KEYDOWN:
			if (VK_ESCAPE == wParam)
				DestroyWindow(hwnd);
			break;


		// ----------------------- display our minimal info
		case WM_PAINT:
		{
			PAINTSTRUCT ps;
			HDC         hdc;
			RECT        rc;
			hdc = BeginPaint(hwnd, &ps);

			GetClientRect(hwnd, &rc);
			SetTextColor(hdc, RGB(240,240,96));
			SetBkMode(hdc, TRANSPARENT);
			DrawText(hdc, pWindowText, -1, &rc, DT_CENTER|DT_SINGLELINE|DT_VCENTER);

			EndPaint(hwnd, &ps);
			break;
		}

		// ----------------------- let windows do all other stuff
		default:
			return DefWindowProc(hwnd, message, wParam, lParam);
	}
	return 0;
}

//+---------------------------------------------------------------------------

int APIENTRY WinMain(
				HINSTANCE hInstance,
				HINSTANCE hPrevInstance,
				LPSTR     lpCmdLine,
				int       nCmdShow)
{
	MSG msg;

	WNDCLASS wc;

	HWND hwnd;

	// Fill in window class structure with parameters that describe
	// the main window.

	ZeroMemory(&wc, sizeof wc);
	wc.hInstance     = hInstance;
	wc.lpszClassName = szAppName;
	wc.lpfnWndProc   = (WNDPROC)WndProc;
	wc.style         = CS_DBLCLKS|CS_VREDRAW|CS_HREDRAW;
	wc.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
	wc.hIcon         = LoadIcon(NULL, IDI_APPLICATION);
	wc.hCursor       = LoadCursor(NULL, IDC_ARROW);

	if (FALSE == RegisterClass(&wc)) return 0;

	// create the browser
	hwnd = CreateWindow(
		szAppName,
		szTitle,
		WS_OVERLAPPEDWINDOW|WS_VISIBLE,
		CW_USEDEFAULT,
		CW_USEDEFAULT,
		360,//CW_USEDEFAULT,
		240,//CW_USEDEFAULT,
		0,
		0,
		g_hInst,
		0);

	if (NULL == hwnd) return 0;

	pWindowText = lpCmdLine[0] ? lpCmdLine : "Hello Windows!";

	// Main message loop:
	while (GetMessage(&msg, NULL, 0, 0) > 0)
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	return msg.wParam;
}

//+---------------------------------------------------------------------------

void CenterWindow(HWND hwnd_self)
{
	RECT rw_self, rc_parent, rw_parent; HWND hwnd_parent;
	hwnd_parent = GetParent(hwnd_self);
	if (NULL==hwnd_parent) hwnd_parent = GetDesktopWindow();
	GetWindowRect(hwnd_parent, &rw_parent);
	GetClientRect(hwnd_parent, &rc_parent);
	GetWindowRect(hwnd_self, &rw_self);
	SetWindowPos(hwnd_self, NULL,
		rw_parent.left + (rc_parent.right + rw_self.left - rw_self.right) / 2,
		rw_parent.top  + (rc_parent.bottom + rw_self.top - rw_self.bottom) / 2,
		0, 0,
		SWP_NOSIZE|SWP_NOZORDER|SWP_NOACTIVATE
		);
}
