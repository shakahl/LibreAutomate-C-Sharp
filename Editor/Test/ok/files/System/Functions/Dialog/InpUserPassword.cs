 /
function# str&user str&password [$title] [hwndOwner]

 Shows dialog for user name and password.
 Returns 1 on OK, 0 on Cancel.

 user - receives user name.
 password - receives password.
 title - dialog title bar text.
 hwndOwner (QM 2.3.4) - handle of owner window or 0.

 EXAMPLE
 str u p
 if(!InpUserPassword(u p)) ret
 out u
 out p


str dd=
 BEGIN DIALOG
 2 "" 0x90C80A4A 0x100 0 0 205 76 ""
 5 Static 0x54000000 0x0 4 10 48 12 "User name"
 3 Edit 0x54030080 0x200 56 10 142 14 ""
 6 Static 0x54000000 0x0 4 32 48 12 "Password"
 4 Edit 0x54030020 0x200 56 32 142 14 ""
 1 Button 0x54030001 0x4 56 58 48 14 "OK"
 2 Button 0x54030000 0x4 108 58 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2010801 "" ""

str controls = "0 3 4"
str f e3 e4

f=iif(!empty(title) title "QM - user name and password")

if(!ShowDialog(dd 0 &controls hwndOwner)) ret

user=e3
password.__set_secure
password=e4
ret 1
