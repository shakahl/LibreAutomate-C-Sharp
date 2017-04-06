function'str [resultsLimit]

 Gets a list of all the pastes created by a user.
 Returns XML text like "<paste>...</paste>...".  <link "http://pastebin.com/api">Example XML</link>.
 Also can return "No pastes found.".

 resultsLimit - default 50. Min 1, max 1000.

 REMARKS
 Need to call Login before.


Http h.Connect("pastebin.com")
h.PostAdd("api_option" "list")
h.PostAdd("api_dev_key" _devKey)
h.PostAdd("api_user_key" _userKey)
if(resultsLimit) h.PostAdd("api_results_limit" resultsLimit)

str R
if(!h.PostFormData("api/api_post.php" 0 R)) end "failed"
if(R.beg("Bad API")) end R
ret R
