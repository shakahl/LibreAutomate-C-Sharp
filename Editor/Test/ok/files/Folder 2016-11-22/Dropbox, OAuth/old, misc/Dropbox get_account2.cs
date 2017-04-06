out

str token=DropboxAuthorize("kuacy0sv1zqrebo" "mgl8i88y880qxcv")

typelib WinHttp {662901FC-6951-4854-9EB2-D9A2570F2B2E} 5.1
WinHttp.WinHttpRequest r._create
r.Open("POST" "https://api.dropboxapi.com/2/users/get_account")
r.SetRequestHeader("Authorization" F"Bearer {token}")
r.SetRequestHeader("Content-Type" "application/json")
str body=
 {
     "account_id": "dbid:AADCHJsswkkelgmwvOzwlxkXA6X67GvmfNw"
 }
r.Send(body)
ARRAY(byte) a=r.ResponseBody
str s.fromn(&a[0] a.len)

out s
