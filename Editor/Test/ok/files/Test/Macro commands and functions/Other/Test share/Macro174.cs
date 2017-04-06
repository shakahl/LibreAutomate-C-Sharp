 Set status bar text
int hwnd=child("" "msctls_statusbar32" "Notepad" 0x1)
str text="text from QM"
strcpy(+share text)
SendMessage(hwnd WINAPI.SB_SETTEXTA 0 share(hwnd))
