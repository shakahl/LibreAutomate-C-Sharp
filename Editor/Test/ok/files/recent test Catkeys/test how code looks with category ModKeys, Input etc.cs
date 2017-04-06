category ModKeys : None, Shift, Ctrl, Alt, CtrlShift, CtrlAlt, ShiftAlt, CtrlAltShift, Win, WinShift, WinCtrl, WinAlt, WinCtrlShift, WinCtrlAlt, WinShiftAlt, WinCtrlAltShift

category lnput :
	Keys, Text, CopyText, Paste, KeyDown, KeyUp, KeyIsPressed, KeyToggle, KeyIsToggled,
	GetTextCursorXY, BlockUserInput, EnterPassword, KeysToArray,
	RemapKeys, DisableCapsLock
	MouseMove, Click, DoubleClick, RightClick, MiddleClick, X1Click, X2Click, MouseWheel,
	MouseDrag, MouseRestoreXY, MouseIsButtonPressed, MouseGetX, MouseGetY, MouseGetXY, MouseGetCursorId
	ModifierKeys

category Input :
	Keys, Text, CopyText, Paste, KeyDown, KeyUp, KeyIsPressed, KeyToggle, KeyIsToggled,
	GetTextCursorXY, BlockUserInput, EnterPassword, KeysToArray,
	RemapKeys, DisableCapsLock
	ModifierKeys

category Mouse :
	Move, Click, DoubleClick, RightClick, MiddleClick, X1Click, X2Click, Wheel,
	Drag, RestoreXY, IsButtonPressed, GetX, GetY, GetXY, GetCursorId

#if 0
Input
lnput
Mouse

switch(Input.ModifierKeys) {
case ModKeys.None:
	...
	break;
case ModKeys.Ctrl:
	...
	break;
case ModKeys.CtrlShift:
	...
	break;

Click overloads:
()
(x y [f])
(x y w [f])
(f)
f - flags: enum MouseFlag { Down, Up, NonClient, Relaxed, Restore, FromCurrentXY, FromLastXY, RightToLeft, BottomToTop }
Ex: Click(MouseFlag.Down);

x and y are of class Coord. It accepts and holds int (pixels) or double (fraction).
w is of class WinElem, which can be Win, Elem (Acc etc) or other type of UI object. Or use overloads.

InputTo.Keys(w, keys);
Input.SendTo(w).Keys(keys);
w.Send.Keys(keys);
w.Send.Click(x y [f])
w.Input.Keys(keys);
