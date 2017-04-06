function hwnd idObject idChild

 Gets object from event, and initializes this variable.

 hwnd idObject idChild - hwnd idObject idChild.

 REMARKS
 Use this function in macros that have accessible object trigger. Gets the object that triggered the macro.
 Also can use in hook function of SetWinEventHook.

 Added in: QM 2.3.3.


a=0; elem=0
VARIANT v
if(AccessibleObjectFromEvent(hwnd idObject idChild &a &v)) end ERR_OBJECTGET
elem=v.lVal
