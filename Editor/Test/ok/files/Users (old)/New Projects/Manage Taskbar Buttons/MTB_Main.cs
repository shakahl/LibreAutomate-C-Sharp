 Adds tray icon and manages hidden windows.
 You can run this function using whatever method except calling from a macro.
 MTB_Hide launches this function if not running.


class __CTaskbarButtons -ARRAY(int)a

__CTaskbarButtons+ g_taskbar

atend MTB_Exit 1

AddTrayIcon "window.ico" "Hidden taskbar buttons" "MTB_Menu" "MTB_Menu2"

rep
	wait 60
	g_taskbar.Update
