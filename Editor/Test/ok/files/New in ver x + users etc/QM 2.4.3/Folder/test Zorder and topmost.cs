 int w=win("Untitled - Notepad" "Notepad")
  Zorder w 0 SWP_NOACTIVATE
 Zorder w HWND_NOTOPMOST SWP_NOACTIVATE
  Zorder w 0
int w=win("TBTEST" "QM_tb_test")
 Zorder w 0 SWP_NOACTIVATE
Zorder w GetWindow(_hwndqm GW_HWNDPREV) SWP_NOACTIVATE
