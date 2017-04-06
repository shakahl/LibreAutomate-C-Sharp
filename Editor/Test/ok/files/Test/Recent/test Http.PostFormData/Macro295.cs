out
ARRAY(POSTFIELD) a.create(5); int i
a[i].name="exe1"; a[i].value="$system$\notepad.exe"; a[i].isfile=1; i+1
a[i].name="txt2"; a[i].value="some text 2"; a[i].isfile=0; i+1
a[i].name="userfile"; a[i].value="$desktop$\test.txt"; a[i].isfile=1; i+1
a[i].name="txt"; a[i].value="some text 1"; a[i].isfile=0; i+1
a[i].name="exe2"; a[i].value="$system$\calc.exe"; a[i].isfile=1; i+1

int hwnd=ShowDialog("" 0 0 0 1)
Http h.Connect("www.quickmacros.com"); str r
if(!h.PostFormData("test/test.php" a r "" &PostFormData_progress_dialog hwnd)) end "failed"
out r

DestroyWindow hwnd

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
