ref WINAMP "$Desktop$\wa502_sdk\winamp\wa_ipc.txt"

int hwnd=win("" "Winamp v1.x")
if(!hwnd) ret
SendMessage(hwnd WM_USER 15 121) ;;select track 15 (does not work)
SendMessage(hwnd WM_USER 0 102) ;;play (works)


 WINAPI.COPYDATASTRUCT d.cbData=sizeof(d)
 d.dwData=WINAMP.IPC_ENQUEUEFILE
  d.lpData="Air - Biological"
 d.lpData="F:\MP3\Archive\6-Londinium.mp3"
 SendMessage(hwnd WM_COPYDATA 0 &d)
 SendMessage(hwnd WINAMP.WM_WA_IPC 0 WINAMP.IPC_STARTPLAY)
