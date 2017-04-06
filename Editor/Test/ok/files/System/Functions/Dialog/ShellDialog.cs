 /
function dlg ;;dlg: 0 Run, 1 Find, 2 Find Computer, 3 Shutdown, 4 Taskbar Properties, 5 Help.

 Shows one of Start menu dialogs.
 Does not wait.


Shell32.Shell sh._create

act "+Shell_TrayWnd"; err

sel dlg
	case 0 sh.FileRun
	case 1 sh.FindFiles
	case 2 sh.FindComputer
	case 3 sh.ShutdownWindows
	case 4 sh.TrayProperties
	case 5 sh.Help
	case else end ERR_BADARG
