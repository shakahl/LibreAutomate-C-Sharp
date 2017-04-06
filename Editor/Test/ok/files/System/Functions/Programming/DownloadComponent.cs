function# $localfile $url $description filesizeKB [funconcomplete] [funcparam]

 Downloads a component (ActiveX, dll, etc).
 Returns: 1 success, 0 failed, -1 exists.

 localfile - path to a missing component file. If it already exists, this function returns -1.
 url - URL of file to download. Zip file is automatically extracted to localfile's folder.
 funconcomplete - 0 or address of user-defined function to be called when successfully downloaded.
   The function must begin with:
   function# $localfile funcparam
   Must return 1 if successful, 0 if failed.
   For example, can install the downloaded component.

 REMARKS
 Can be called on a compile-time error, when typelib or dll statement fails.


str s sl.searchpath(localfile "$qm$")
if(sl.len) ret -1
if(findc(localfile '\')<0) sl.from("$qm$\" localfile); else sl=localfile
sl.expandpath

lpstr title="QM - component download"
if(mes(F"Component not found. Download now?[][]Description: {description}[]Required file: {localfile}[]Download from: {url}[]File size: {filesizeKB} KB." title "OC?")!='O') ret

IntGetFile url s 4 0 _hwndqm
if(s.len<filesizeKB-10*1024) end F"size of downloaded file is only {s.len/1024} KB. Expected {filesizeKB} KB." 8

str sd fn.getfilename(url 1)
if(fn.endi(".zip"))
	__TempFile tf.Init(0 fn "" s)
	zip- tf sd.getpath(sl "")
else s.setfile(sl)

lpstr ai
if(funconcomplete)
	if(!call(funconcomplete sl funcparam)) mes F"Failed to install {localfile}." title "x"; ret 0
	ai=" and installed"

mes F"{localfile} successfully downloaded{ai}." title "i"
ret 1
 ge
err+ mes F"Failed to download {localfile}." title "x"
