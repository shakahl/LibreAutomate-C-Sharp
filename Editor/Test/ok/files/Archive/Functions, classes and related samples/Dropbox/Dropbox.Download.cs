function'str $path [$localFile] [str&toVariable]

 Downloads a file from Dropbox.
 Returns Dropbox response string (JSON) containing file info. Error if fails.

 path - file path in Dropbox. Example: "/Homework/math/Matrices.txt". Also can be file id, like "id:xxxxxxxxxxxxxxxxx".
 localFile - local file full path where to save the file. Optional, can be "" if don't need to save.
 toVariable - a variable that receives file data. Optional, can be 0.

 REMARKS
 Uses Dropbox API /download. More info:
 <link>https://www.dropbox.com/developers/documentation/http/documentation#files-download</link>


opt noerrorshere
if(!token.len) end ERR_INIT

WinHttp.WinHttpRequest r._create
r.Open("POST" "https://content.dropboxapi.com/2/files/download")
r.SetRequestHeader("Authorization" F"Bearer {token}")
r.SetRequestHeader("Dropbox-API-Arg" F"{{''path'': ''{path}''}")
r.Send()

str rt=r.GetResponseHeader("Dropbox-API-Result")

if(r.Status!=200) end _s.from(r.StatusText ".  " rt)

VARIANT v

if !empty(localFile)
	v=r.ResponseStream
	if(v.vt!=VT_UNKNOWN or v.punkVal=0) end ERR_FAILED
	IStream u=v.punkVal
	__Stream b.CreateOnFile(localFile STGM_WRITE|STGM_CREATE)
	int na(65000) nr; _s.all(na)
	rep
		u.Read(_s na &nr)
		if(!nr) break
		b.is.Write(_s nr 0)

if &toVariable
	v=r.ResponseBody
	SAFEARRAY* k=v.parray.psa
	if(v.vt=0x2011 and k and k.rgsabound[0].cElements) toVariable.fromn(k.pvData k.rgsabound[0].cElements)
	else toVariable=""

ret rt
