 /
function [$iconfile] [$tooltip] [$onclick] [$onrclick]

 Adds tray icon for current macro.

 iconfile - icon file.
   Can be ico or other. Can be with icon index, like "shell32.dll,8".
   Supports <help #IDP_RESOURCES>macro resources</help> (QM 2.4.1) and exe resources.
 tooltip - tooltip text. Displays max 127 characters.
 onclick - name of macro, function or menu. It will be launched on left click.
   If this function is called from macro, onclick should not be macro. If in exe, must be function.
 onrclick - the same with right click.

 REMARKS
 The tray icon will be automatically deleted when thread ends. To delete before that, use this function with "<delete>" as iconfile.
 You can Ctrl+click it to end macro.
 To have more control, use Tray class. Read <help>Tray help</help>.

 EXAMPLES
 AddTrayIcon "$qm$\mouse.ico" "this is my macro" "FunctionThatRunsWhenIClickTheIcon"
 10
 
 AddTrayIcon "resource:mouse.ico" ;;using resource "mouse.ico" of this macro
 wait -1


#opt nowarnings 1
Tray-- trayicon
sel iconfile
	case "<delete>" trayicon.Delete
	case else trayicon.AddIcon(iconfile tooltip 0 0 0 onclick onrclick)
