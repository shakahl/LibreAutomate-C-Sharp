function'str $path

 Deletes file or folder.
 Returns Dropbox response string (JSON). Error if fails.

 path - file or folder path in Dropbox. Example: "/Homework/math/file.txt".

 REMARKS
 Uses Dropbox API /list_folder. More info:
 <link>https://www.dropbox.com/developers/documentation/http/documentation#files-delete</link>


opt noerrorshere
if(!token.len) end ERR_INIT

WinHttp.WinHttpRequest r._create
r.Open("POST" "https://api.dropboxapi.com/2/files/delete")
r.SetRequestHeader("Authorization" F"Bearer {token}")
r.SetRequestHeader("Content-Type" "application/json")

str data=
F
 {{
 "path": "{path}"
 }

r.Send(data)

str rt=r.ResponseText
if(r.Status!=200) end _s.from(r.StatusText ".  " rt)

ret rt
