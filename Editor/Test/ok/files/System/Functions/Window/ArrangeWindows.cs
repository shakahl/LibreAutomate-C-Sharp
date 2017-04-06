function action [$windowlist] [$name] ;;action: 0 show/hide desktop, 1 minimize all, 2 restore all, 3 cascade, 4 tile horizontal, 5 tile vertical, 6 save, 7 restore, 8 Flip 3D.

 Arranges windows.

 action - see above.
   6 - calls <help>SaveMultiWinPos</help>.
   7 - calls <help>RestoreMultiWinPos</help>.
   8 (QM 2.4.1) - toggles Flip 3D view on/off, like with Win+Tab. Works on Windows 7 with Aero theme.
 windowlist - used with action 6.
 name - used with actions 6 and 7.

 REMARKS
 With action 0, 1 and 2, the function behaves like when you press Win+D, Win+M, Win+Shift+M. Restores Windows in random order.


opt noerrorshere 1
sel action
	case 6 ;;Save
	if(getopt(nargs)<3) ArrangeWindowsOld action windowlist
	else SaveMultiWinPos name windowlist
	ret
	
	case 7 ;;Restore
	if(getopt(nargs)<3) ArrangeWindowsOld action windowlist
	else RestoreMultiWinPos name; ret
	
	case 3 CascadeWindows 0 0 0 0 0 ;;sh.CascadeWindows etc skips admin windows
	case 4 TileWindows 0 1 0 0 0
	case 5 TileWindows 0 0 0 0 0
	
	case else
	Shell32.Shell sh._create
	IDispatch d=sh
	sel action
		case 0 d.ToggleDesktop; err
		case 1 sh.MinimizeAll
		case 2 sh.UndoMinimizeALL
		case 8 if(_winnt>=6) d.WindowSwitcher; err
	
	 note: restores windows in random order, could not find a reliable workaround.

wait -2
