/Dialog_Editor
function $connection $ftpserver $myname $mypassword ~files [$localdir] [$ftpdir] [hwndowner]

 Uploads files to FTP server.
 Before uploading, shows a dialog with specified files.
 In the dialog the user can select files. Only selected files will be uploaded.
 On failure shows the dialog again. Only files that are still not uploaded will be selected. Click OK to retry.
 When all uploaded, shows a message box.

 connection - currently not used. Should be "".
 ftpserver - eg "ftp.my.com"
 myname, mypassword - user name and password.
 localdir - folder on this computer where the files are, eg "c:\my documents\ftp"
 ftpdir - folder on FTP server where to place files, eg "public_html"
 hwndowner - handle of a window or dialog that will be owner of the dialog.
 files - list of files to upload. Example:
    "&index.html[]..\myapp.zip>myapp.zip[][]images\cat.gif"
    Syntax:
       [] (new line) separates files
       & selects the file in the dialog
       > used if destination filename or path is different. Left - local file. Right - FTP file.
       ..\ parent directory


str pw.decrypt(16 mypassword "sample encr key") ;;support encrypted password

str controls = "0 1 3"
str Dlg.from("Upload to " ftpserver)
str Button1 = "OK"
 g1
str ListBox3=files
if(!ShowDialog("FtpUpload" 0 &controls hwndowner)) ret

 BEGIN DIALOG
 0 "" 0x10CF0848 0x100 0 0 200 170 ""
 3 ListBox 0x54230109 0x204 4 4 192 142 ""
 1 Button 0x54030001 0x4 4 152 48 14 "OK"
 2 Button 0x54030000 0x4 58 152 48 14 "Cancel"
 END DIALOG

Ftp f.Connect(ftpserver myname pw)
if(len(localdir)) ChDir localdir
if(len(ftpdir) and f.DirSet(ftpdir)=0) ret
f.SetProgressDialog(1)

int i j; str s.flags=1; str s1.flags=1; str s2.flags=1
str failed.flags=1; failed.all
for i 0 ListBox3.len
	s.getl(files -i); if(ListBox3[i]!='1' or s.len=0) continue
	if(s[0]='&') s.get(s 1)
	j=findc(s '>')
	if(j>=0) s1.left(s j); s2.get(s j+1)
	else s1=s; s2=s
	for(j 0 s1.len) if(s1[j]='/') s1[j]='\'
	for(j 0 s2.len) if(s2[j]='\') s2[j]='/'
	if(f.FilePut(s1 s2)) out "Uploaded: ''%s'' to ''%s''" s1 s2
	else
		out "FTP error: can't upload ''%s'' to ''%s''" s1 s2
		failed+"&"; failed+s; failed+"[]"
 disc
if(failed.len) failed.fix(failed.len-2); files=failed; Button1="Retry"; goto g1
mes "FTP session finished"
err+ mes _error.description "FTP error"
