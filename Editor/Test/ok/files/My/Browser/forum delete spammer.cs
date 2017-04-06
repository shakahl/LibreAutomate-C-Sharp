 Run this when in firefox is displayed view topic page with a spam post.
 Asks you to click spammer user profile link.
 If you click it, deletes the spammer and all his posts.

int w=win(" - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
act w
Acc a
 goto g1 ;;debug

 click the user manually, to avoid accidental deleting another user
_monitor=w
OnScreenDisplay "click the spammer user profile link within 10 s" 10 0 0 0 0 0 0 "forum_delete_spammer"
wait 10 ML; err OnScreenDisplay "timeout" 3 0 0 0 0 0 0 "forum_delete_spammer"; ret
OsdHide "forum_delete_spammer"

a.FindFF(w "A" "Administrate user" "" 0x1001 20)
a.DoDefaultAction
 g1
 ----
a.FindFF(w "" "Here you can change your users information and certain specific options." "" 0x1001 60)
key CE
a.FindFF(w "SELECT" "" "name=delete_type[]id=delete_user" 0x1004)
a.Select(1)
key D
a.Navigate("parent previous first") ;;checkbox
a.DoDefaultAction
a.Navigate("parent previous parent next first") ;;Submit
a.DoDefaultAction
a.FindFF(w "INPUT" "" "value=Yes[]name=confirm" 0x1004 30)
a.DoDefaultAction
