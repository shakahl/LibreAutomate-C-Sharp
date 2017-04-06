act "Options"
spe

Acc a=acc("Mouse buttons" "CHECKBUTTON" "Options" "Button" "" 0x1001)
 a.Mouse(1)
a.DoDefaultAction

Acc a2=acc("Mouse drag" "CHECKBUTTON" "Options" "Button" "" 0x1001)
if(a2.State&STATE_SYSTEM_UNAVAILABLE) out "disabled"; else out "enabled"
