 Does not work.
 Then just the first mouse click does not work.
 The codes are not clearly documented what they do, and I did not find on the internet.

 int w=_hwndqm
int w=win
 SendMessage(w WM_SYSCOMMAND SC_NEXTWINDOW 0)
 SendMessage(w WM_SYSCOMMAND SC_PREVWINDOW 0)
 PostMessage(w WM_SYSCOMMAND SC_NEXTWINDOW 0)
PostMessage(w WM_SYSCOMMAND SC_PREVWINDOW 0)

 PostMessage(w WM_SYSCOMMAND SC_TASKLIST 0)
