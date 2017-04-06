out

str accessToken="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" ;;you can generate it in your Dropbox app settings page
str accountId="yyyyyyyyyyyyyyyyyyyyyyyy" ;;see next macro

typelib WinHttp {662901FC-6951-4854-9EB2-D9A2570F2B2E} 5.1
WinHttp.WinHttpRequest r._create
r.Open("POST" "https://api.dropboxapi.com/2/users/get_account")
r.SetRequestHeader("Authorization" F"Bearer {accessToken}")
r.SetRequestHeader("Content-Type" "application/json")
str body=
F
 {{
     "account_id": "dbid:{accountId}"
 }
r.Send(body)
ARRAY(byte) a=r.ResponseBody
str s.fromn(&a[0] a.len)

out s
