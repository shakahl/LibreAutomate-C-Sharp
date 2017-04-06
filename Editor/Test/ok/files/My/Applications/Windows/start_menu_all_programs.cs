 When I open Windows 7 Start menu, opens All Programs.
 Probably works on Vista too, but not tested.


if(_winver<0x600) ret ;;run only on Vista and later
int hwnd=val(_command)
Acc a=acc("All programs" "MENUITEM" hwnd "Button" "" 0x1001 0 0 "" 10)
 err ret
a.DoDefaultAction
act child("" "Search Box" hwnd) ;;focus Search box
