 int hwnd
 hwnd=win("MSDN Library - January 2002 - _msize")
 act hwnd
 hwnd=id(2454 "MSDN Library - January 2002")
 act "Font"
 int hwnd=child("Courier New" "Edit" "Font" 0x5)
 act hwnd
 AccSelect(SELFLAG_TAKEFOCUS "Look for:" "COMBOBOX" "MSDN Library - January 2002 - _msize" "ComboBox" "" 0x1001)

 int hwnd=child(303 "14:4\d" "TrayClockWClass" "+Shell_TrayWnd" 0x200)
 int hwnd=id(304 "+Shell_TrayWnd")
 
 act "QM Help"
 act id(3003 "QM Help")
 act
 clo id(3003 "QM Help")
 clo+ win("QM Help" "HH Parent")
 max win("Notepad" "Notepad")
 min
 res "Untitled - Notepad"
 hid id(15 "Untitled - Notepad")
 hid- h
 ont "Untitled - Notepad"
 ont- "Untitled - Notepad"
 mov 10 500 "Untitled - Notepad"
 mov 500 0 "" 2
 mov 0 500 "" 5
 mov 0 20 id(15 "Untitled - Notepad") 1
 siz 500 200 "Untitled - Notepad"
 siz 300 0 "Untitled - Notepad" 2
 siz 0 400 "Untitled - Notepad" 1
 MoveWindow win("Untitled - Notepad") 10 50 100 500 1
 GetWinXY win("Untitled - Notepad") x y cx cy
 Zorder(win("Untitled - Notepad"))
 Zorder(h2)
 Zorder(win("Untitled - Notepad") HWND_BOTTOM)
 ifa("Untitled - Notepad")
	 out 1
 ifi-("Untitled - Notepad")
	 out 1
 if(IsIconic(win("Untitled - Notepad")))
	 bee
 if(!IsZoomed(win("Untitled - Notepad")))
	 bee
 if(IsWindowVisible(win()))
	 bee
 if(!IsWindowEnabled(win("Untitled - Notepad")))
	 out 1
 _s="ddd"; _s.setwintext(win("Untitled - Notepad"))
 _s="hh"; _s.setwintext(id(15 "ddd"))
 str a.getwintext(win("ddd"))
 str a.getwinclass(id(15 "ddd"))
 str a.getwinexe(win("MSDN Library - January 2002 - ShowWindow"))
 out a
 int a=GetWinId(child("\d\d:\d\d" "TrayClockWClass" "+Shell_TrayWnd" 0x201))
 out a
 int stl=GetWinStyle(win("ddd"))
 int a=GetWinStyle(win("STICKY TOOLBAR") 1)
 int a=GetParent(id(15 "ddd"))
 int a=GetAncestor(child("Notification Area" "ToolbarWindow32" "+Shell_TrayWnd" 0x1) 2)
 int a=GetWindow(win("" "Shell_TrayWnd") GW_CHILD)
 int a=GetWindow(win() GW_HWNDFIRST)
 int a=GetWindow(win("Notepad") GW_OWNER)
 int a=child()
 ArrangeWindows(0)
 ArrangeWindows(1)
 ArrangeWindows(2)
 ArrangeWindows(3)
 ArrangeWindows(4)
 ArrangeWindows(5)
 ArrangeWindows(6 "Untitled - Notepad[]MSDN Library - January 2002 - ShowWindow[]app - Microsoft Visual C++ [design] - RunWin.cpp[]Reviews and free downloads at Download.com - Microsoft Internet Explorer - [Working Offline]" "testing")
 15
 ArrangeWindows(7 "" "testing")

act "hh - Microsoft Visual C++"; err ErrMsg(2)


 int h2=GetWindow(id(1136 "Font") GW_HWNDNEXT)
