str ftpServer="ftp.quickmacros.com"
str ftpUsername="quickmac"
str ftpPassword="*"
str ftpDirectory="public_html/helpsearch"
str urlWithoutFilename="http://www.quickmacros.com/helpsearch"
str parameters=
 qmver=2020106
 qtype=0
 qwords=move window
 comments=[move window]
int usePOST=0

 Notes:
 This is for HTML Kit. To use with PSPad, just change the win line below.
 When using with HTML Kit, two conditions required:
   Check 'Display full path names in editor title bar' in Preferences.
   The document window must be maximized, otherwise the path is displayed in it instead of in main window.


 out

 close all preview windows
rep
	int h=win("PhpDebug" "#32770" "qm"); if(!h) break
	clo h

 get editor window handle
 int w1=win("PSPad" "TfPSPad.UnicodeClass") ;;use this for PSPad
int w1=win("HTML-Kit" "TChamiHKMainForm") ;;use this for HTML Kit
if(!w1) mes- "Window not found"
act w1
 save
key Cs
 get file path and name
 g1
str s.getwintext(w1) fn
if(findc(s '*')>0) 0.01; goto g1 ;;if still not saved, wait
s.replacerx(".+\[(.+?)\]$" "$1" 4)
fn.getfilename(s 1)
 out s; ret

 connect to ftp server
OnScreenDisplay "Connecting to FTP server..." -1 0 0 0 0 0 8
 g2
Ftp f.Connect(ftpServer ftpUsername ftpPassword) ;;1s
err if(mes("Connecting failed." "" "RC")='R') goto g2; else ret
 set directory
if(ftpDirectory.len)
	ftpDirectory.findreplace("\" "/")
	if(!ftpDirectory.beg("/")) ftpDirectory-"/"
	if(!f.DirSet(ftpDirectory)) mes- "Failed to set FTP directory"
 upload the file, replacing if exists
OsdHide; OnScreenDisplay "Uploading..." -1 0 0 0 0 0 8
 g3
if(!f.FilePut(s fn)) ;;2s
	if(mes("Uploading failed." "" "RC")='R') goto g3; else ret

OsdHide; OnScreenDisplay "Executing..." -1 0 0 0 0 0 8

 format url
if(!urlWithoutFilename.end("/")) urlWithoutFilename+"/"
if(parameters.len)
	str sp s0 s1 s2
	foreach s0 parameters
		if(tok(s0 &s1 2 "=" 2)=2) sp.formata("&%s=%s" s1.escape(9) s2.escape(9))
	sp[0]='?'
s.from(urlWithoutFilename fn)
 out sp

 download to temporary file
str sf="$temp qm$\qmphpdebug.htm"
 g4
err-
if(usePOST and sp.len)
	IntPost s sp+1 _s
	_s.setfile(sf)
else
	s+sp
	IntGetFile s sf 16
err+ if(mes("Executing failed." "" "RC")='R') goto g4; else ret
 preview
mac "PhpDebugDlg" sf
