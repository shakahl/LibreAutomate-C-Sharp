function'str $localFile $path $mode [flags] [$content_type] ;;flags: 1 autoRename=true, 2 mute=true

 Uploads a file to Dropbox.
 Returns Dropbox response string (JSON). Error if fails.

 localFile - local file path.
 path - path in the user's Dropbox to save the file. Example: "/Homework/math/Matrices.txt".
 mode - "add" (auto-rename), "overwrite" or "update".
 content_type - Content-Type header. Default: "application/octet-stream".

 REMARKS
 Uses Dropbox API /upload. More info:
 <link>https://www.dropbox.com/developers/documentation/http/documentation#files-upload</link>
 Do not use this to upload a file larger than 150 MB.


opt noerrorshere
if(!token.len) end ERR_INIT

__Stream b.CreateOnFile(localFile STGM_READ)

WinHttp.WinHttpRequest r._create
r.Open("POST" "https://content.dropboxapi.com/2/files/upload")
r.SetRequestHeader("Authorization" F"Bearer {token}")
r.SetRequestHeader("Content-Type" iif(empty(content_type) "application/octet-stream" content_type)) ;;note: use IStream, or will append Charset=UTF-8 and fail

str arg=
F
 {{
 "path": "{path}",
 "mode": "{mode}",
 "autorename": {iif(flags&1 "true" "false")},
 "mute": {iif(flags&2 "true" "false")}
 }
arg.findreplace("[]" " ")
r.SetRequestHeader("Dropbox-API-Arg" arg)

r.Send(b)

str rt=r.ResponseText
if(r.Status!=200) end _s.from(r.StatusText ".  " rt)

ret rt
