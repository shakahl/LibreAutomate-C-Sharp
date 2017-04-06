 To download a file you can use function IntGetFile.
 Sometimes it is better to use functions of Http or Ftp class instead. For example, download multiple files without reconnecting, or when required password.
 Most Http functions return 1 on success, 0 if failed.
 If a function failed, error description in most cases is stored in member variable lasterror.
 Note: Http functions don't support Unicode. Support Unicode only in local file names.

 EXAMPLES

 Download file to variable
str s
IntGetFile "http://www.quickmacros.com/index.html" s
ShowText "" s

 Download file to file, show progress dialog
str localfile="$desktop$\quickmac.ex_"
IntGetFile "http://www.quickmacros.com/quickmac.exe" localfile 16 0 1
ren- localfile "$desktop$\quickmac.exe"

 Download multiple files using Http class
str files=
 index.html
 images\xxx.gif
str sf sfloc localfolder="$desktop$\qm Http"
mkdir localfolder
mkdir _s.from(localfolder "\images")
Http h.Connect("www.xxxxxx.com")
h.SetProgressDialog(1)
foreach sf files
	sfloc.from(localfolder "\" sf)
	h.Get(sf sfloc 16)
