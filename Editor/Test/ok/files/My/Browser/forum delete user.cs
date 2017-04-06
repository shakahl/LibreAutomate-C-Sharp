 Run this when in firefox is displayed Members list (or ACP/Users&Groups/[Find a member], but it must be opened in main window, not popup (copy/paste URL)).
 Asks you to middle click user profile link.
 If you click it, deletes the user and his posts.

int w=win(" - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
act w
Acc a
 goto g1 ;;debug

rep
	 click the user manually, to avoid accidental deleting another user
	_monitor=w
	OnScreenDisplay "middle click the user profile link within 30 s" 30 0 0 0 0 0 8 "forum_delete_user"
	wait 30 MM; err OnScreenDisplay "timeout" 3 0 0 0 0 0 0 "forum_delete_user"; ret
	OsdHide "forum_delete_user"
	1
	 g1
	a.FindFF(w "A" "Administrate user" "" 0x1001 20)
	a.DoDefaultAction
	 ----
	a.FindFF(w "" "Here you can change your users information and certain specific options." "" 0x1001 60)
	key CE
	a.FindFF(w "SELECT" "" "name=delete_type[]id=delete_user" 0x1004 2)
	a.Select(1)
	key D
	a.Navigate("parent previous first") ;;checkbox
	a.DoDefaultAction
	a.Navigate("parent previous parent next first") ;;Submit
	 a.showRECT; ret
	a.DoDefaultAction
	a.FindFF(w "INPUT" "" "value=Yes[]name=confirm" 0x1004 30)
	a.DoDefaultAction
	 ----
	a.FindFF(w "" "User deleted successfully." "" 0x1001 10)
	key Cw ;;close tab
