out
 files
ARRAY(str) af.create(2)
af[0]="$system$\notepad.exe"
af[1]="$system$\calc.exe"
 callback function data
type PFD_CALLBACK2 hwnd nbTotal nbSent
PFD_CALLBACK2 c
 get total size
Dir d; int i
for(i 0 af.len) if(d.dir(af[i])) c.nbTotal+d.FileSize; else end "file does not exist"
 show dialog, connect
c.hwnd=ShowDialog("" 0 0 0 1)
Http h.Connect("www.quickmacros.com"); str r
 post
ARRAY(POSTFIELD) a.create(1); a[0].name="userfile"; a[0].isfile=1
for i 0 af.len
	a[0].value=af[i]
	if(!h.PostFormData("form.php" a r "" &PostFormData_progress_dialog3 &c)) end "failed"
	out r
 results
out "Total size of uploaded files is %i KB" c.nbTotal/1024

DestroyWindow c.hwnd

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 222 80 "Form"
 2 Button 0x54030000 0x4 168 62 48 14 "Cancel"
 3 msctls_progress32 0x54000000 0x0 30 6 186 12 ""
 4 msctls_progress32 0x54000000 0x0 30 24 186 12 ""
 5 Static 0x54000000 0x0 4 6 24 12 "Total"
 6 Static 0x54000000 0x0 4 24 22 12 "File"
 7 Static 0x54000000 0x0 4 42 212 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""
