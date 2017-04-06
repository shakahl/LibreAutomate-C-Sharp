 \Dialog_Editor
function# $localfile [$ftpfile] [flags]

 Uploads localfile to ftp server. The same as Ftp.FilePutWithCallback, but shows progress dialog.
 Returns 1 on success, -1 on Cancel. On failure returns 0.

 See also: <Ftp.FilePutWithCallback>

 EXAMPLE
 Ftp f.Connect("ftp.mysite.com" "user" "password")
 f.DirSet("public_html")
 if(!f.FilePutWithProgress("$desktop$\test.exe" "" 3)) mes- "failed"


str controls = "4"
str e4
e4=localfile
int h=ShowDialog("" 0 &controls 0 1)
int r=FilePutWithCallback(localfile ftpfile flags &Ftp_FilePutProgressCb h)
clo h; err
ret r

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 194 68 "QM - Ftp Upload"
 3 msctls_progress32 0x54000000 0x4 6 34 184 12 ""
 2 Button 0x54030000 0x0 68 50 48 14 "Cancel"
 4 Edit 0x54030880 0x0 6 6 184 12 ""
 5 Static 0x54000000 0x0 6 20 184 13 "Preparing to upload..."
 END DIALOG
 DIALOG EDITOR: "" 0x202000B "" ""
