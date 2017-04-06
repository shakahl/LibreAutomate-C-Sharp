 /
function'__Tooltip* hDlg

 Call this function from dialog procedure if you want to modify its tooltips.
 Returns address of an internal __Tooltip variable.

 You can call member functions of the variable, and/or send messages directly to the tooltip control (reference in MSDN).
 Member htt is tooltip control handle. It is 0 if there was no tooltips in dialog definition and Create() not called.


__DIALOG* d=+GetProp(hDlg +__atom_dialogdata)
if(!d) ret
if(!d.tt) d.tt._new
ret d.tt
