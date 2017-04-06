 /
function disable [reEnableAfterS] ;;disable: 0 enable, 1 disable

 Disables, enables or temporarily disables the Ctrl+Alt+Delete requirement at Windows logon/unlock.
 Error if fails.

 reEnableAfterS - number of seconds to disable. Then will enable again. QM must be running at that time.

 NOTES
 QM must be running as administrator.
 If the Windows setting already matches <b>disable</b>, this function does nothing.

 EXAMPLE
 DisableWindowsLogonCtrlAltDelete 1 10 ;;disables the Ctrl+Alt+Delete screen for 10 s
 shutdown 6 ;;lock


if(!disable) tim
if(!IsUserAdmin) end "QM must be running as administrator"

str rk="SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"
int DisableCAD
rget DisableCAD "DisableCAD" rk HKEY_LOCAL_MACHINE
if((DisableCAD!0)=(disable!0)) ret

if(!rset(disable!0 "DisableCAD" rk HKEY_LOCAL_MACHINE)) end "failed"
if(disable and reEnableAfterS) tim reEnableAfterS DisableWindowsLogonCtrlAltDelete
