function'str $pasteKey

 Gets the text of a users paste.

 pasteKey - a paste key. You can see it in a paste URL, or in <paste_key> that you get with List().

 REMARKS
 Need to call Login before.


Http h.Connect("pastebin.com")
h.PostAdd("api_option" "show_paste")
h.PostAdd("api_dev_key" _devKey)
h.PostAdd("api_user_key" _userKey)
h.PostAdd("api_paste_key" pasteKey)

str R
if(!h.PostFormData("api/api_raw.php" 0 R)) end "failed"
if(R.beg("Bad API")) end R
ret R
