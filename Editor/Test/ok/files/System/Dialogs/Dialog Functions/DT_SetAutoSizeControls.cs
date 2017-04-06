 /
function hDlg $controls

 Sets to move/resize controls when resizing dialog.

 hDlg - hDlg.
 controls - space-separated list of controls.
   Example: "1m 2m 3sh 4mv 5s".
   Syntax for a control: IdActionDirection.
     Id - control id.
     Action - m (move) or s (resize).
     Direction - h (horizontally) or v (vertically) or none (horiz and vert).

 REMARKS
 Call this function in dialog procedure, under case WM_INITDIALOG. Don't need more code.
 To make dialog resizable, in dialog editor add WS_THICKFRAME style. Optionally add WS_MAXIMIZEBOX, WS_MINIMIZEBOX.
 This function can be used in any window procedure, not only dialog. Call once, after creating controls.

 Added in: QM 2.4.1.

 EXAMPLE
 	case WM_INITDIALOG
 	DT_SetAutoSizeControls hDlg "3s"


__SetAutoSizeControls hDlg controls
