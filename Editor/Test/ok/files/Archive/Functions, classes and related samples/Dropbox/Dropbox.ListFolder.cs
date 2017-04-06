function'str $path [flags] ;;flags: 1 recursive=true, 2 include_media_info=true, 4 include_deleted=true, 8 include_has_explicit_shared_members=true

 Gets folder contents.
 Returns Dropbox response string (JSON). Error if fails.

 path - folder path in Dropbox. Examples: "/Homework/math", "".
 flags - see above and <link>https://www.dropbox.com/developers/documentation/http/documentation#files-list_folder</link>.

 REMARKS
 Uses Dropbox API /list_folder. More info:
 <link>https://www.dropbox.com/developers/documentation/http/documentation#files-list_folder</link>


opt noerrorshere
if(!token.len) end ERR_INIT

WinHttp.WinHttpRequest r._create
r.Open("POST" "https://api.dropboxapi.com/2/files/list_folder")
r.SetRequestHeader("Authorization" F"Bearer {token}")
r.SetRequestHeader("Content-Type" "application/json")

str data=
F
 {{
 "path": "{path}",
 "recursive": {iif(flags&1 "true" "false")},
 "include_media_info": {iif(flags&2 "true" "false")},
 "include_deleted": {iif(flags&4 "true" "false")},
 "include_has_explicit_shared_members": {iif(flags&8 "true" "false")}
 }

r.Send(data)

str rt=r.ResponseText
if(r.Status!=200) end _s.from(r.StatusText ".  " rt)

ret rt
