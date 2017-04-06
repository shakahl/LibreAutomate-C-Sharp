function $user $password

 Logs in as a user.
 Optional for Paste(), required for other functions.

 user - pastebin user name.
 password - password.


Http h.Connect("pastebin.com")
h.PostAdd("api_dev_key" _devKey)
h.PostAdd("api_user_name" user)
h.PostAdd("api_user_password" password)

if(!h.PostFormData("api/api_login.php" 0 _s)) end "failed"
if(_s.beg("Bad API")) end _s
_userKey=_s
