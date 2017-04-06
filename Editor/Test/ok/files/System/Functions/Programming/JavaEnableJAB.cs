function! [action] ;;action: 0 select, 1 enable for this user, 2 enable all users, 3 disable for this user, 4 disable all users

 Enables or disables Java Access Bridge.
 Returns: 1 success, 0 failed.
 To enable/disable for all users, QM must be running as admin.
 Added in: QM 2.4.2.


if !action
	action=ListDialog("Enable for this user[]Enable for all users[]Disable for this user[]Disable for all users" "Enable or disable Java Access Bridge." "Quick Macros")
	if(!action) ret

str sf sd se endis=iif(action<3 "en" "dis")
if(!TO_JavaGetDir(sf)) se="Cannot find Java. Make sure it is installed."; goto ge

sel action
	case [1,3]
	sf+"\bin\jabswitch.exe"
	if(!FileExists(sf)) se="Cannot find jabswitch.exe. Should be installed Java 7.6 or later."; goto ge
	if(RunConsole2(F"''{sf}'' -{endis}able" _s)) goto ge
	out _s.trim
	if(action=3) out "Also may need to disable for all users."
	
	case [2,4]
	sf+"\lib\accessibility.properties"
	sd.getfile(sf)
	if(!sd.replacerx("(?m)^#?(assistive_technologies=com.sun.java.accessibility.AccessBridge)" "#$1"+(action=2) 4)) goto ge
	sd.replacerx("(?m)^#?(screen_magnifier_present=true)" "#$1"+(action=2) 4)
	sd.setfile(sf); err se=F"{_error.description}[][9]Make sure that this program is running as administrator."; goto ge
	out F"The Java Access Bridge has been {endis}abled."
	if(action=4) out "Also may need to disable for this user."
	else out "Note: now users cannot disable separately."
	
	case else end ERR_BADARG

out "Restart Java applications to apply the new settings."
ret 1
err+ se=_error.description
 ge
if(se.len) se-"[][9]"
out F"Failed to {endis}able Java Access Bridge.{se}"; ret
