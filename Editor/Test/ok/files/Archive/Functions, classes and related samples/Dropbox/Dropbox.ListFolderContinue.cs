function'str $cursor

 Continues to get folder contents after calling ListFolder() (or ListFolderContinue()) that returned "has_more": true.
 Returns Dropbox response string (JSON). Error if fails.

 cursor - cursor value from JSON returned by ListFolder() or ListFolderContinue().

 REMARKS
 Uses Dropbox API /list_folder/continue. More info:
 <link>https://www.dropbox.com/developers/documentation/http/documentation#files-list_folder-continue</link>

 This function is not tested, I don't have so many files. Somebody says the limit is 2000 files.


opt noerrorshere
if(!token.len) end ERR_INIT

WinHttp.WinHttpRequest r._create
r.Open("POST" "https://api.dropboxapi.com/2/files/list_folder/continue")
r.SetRequestHeader("Authorization" F"Bearer {token}")
r.SetRequestHeader("Content-Type" "application/json")

str data=
F
 {{
 "cursor": "{cursor}",
 }

r.Send(data)

str rt=r.ResponseText
if(r.Status!=200) end _s.from(r.StatusText ".  " rt)

ret rt
