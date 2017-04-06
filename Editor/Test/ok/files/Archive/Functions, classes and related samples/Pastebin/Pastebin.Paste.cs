function'str $text [$name] [private] [$expireDate] [$format] ;;private: 0 public, 1 unlisted, 2 private.  expireDate: N, 10M, 1H, 1D, 1W, 2W, 1M

 Posts a new paste.
 Returns the URL of the new paste.

 text - the text of your paste.
 name - the name/title of your paste.
 private - makes the paste public or private.
 expireDate - sets the expiration date of your paste. <link "http://pastebin.com/api">Reference</link>. Default is never (N).
 format - the syntax highlighting value. <link "http://pastebin.com/api">Reference</link>.

 REMARKS
 If called Login before, pastes as that user, else as a guest.


Http h.Connect("pastebin.com")
h.PostAdd("api_option" "paste")
h.PostAdd("api_dev_key" _devKey)
h.PostAdd("api_user_key" _userKey)
if(!empty(name)) h.PostAdd("api_paste_name" name)
h.PostAdd("api_paste_private" private)
if(!empty(expireDate)) h.PostAdd("api_paste_expire_date" expireDate)
if(!empty(format)) h.PostAdd("api_paste_format" format)
h.PostAdd("api_paste_code" text)

str R
if(!h.PostFormData("api/api_post.php" 0 R)) end "failed"
if(R.beg("Bad API")) end R
ret R
