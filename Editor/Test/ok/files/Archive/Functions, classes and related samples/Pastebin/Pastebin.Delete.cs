function $pasteKey

 Deletes a paste.

 pasteKey - a paste key. You can see it in a paste URL, or in <paste_key> that you get with List().

 REMARKS
 Need to call Login before.


Http h.Connect("pastebin.com")
h.PostAdd("api_option" "delete")
h.PostAdd("api_dev_key" _devKey)
h.PostAdd("api_user_key" _userKey)
h.PostAdd("api_paste_key" pasteKey)

if(!h.PostFormData("api/api_post.php" 0 _s)) end "failed"
if(_s.beg("Bad API")) end _s
