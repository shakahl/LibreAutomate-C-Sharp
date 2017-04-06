out
ARRAY(POSTFIELD) a.create(1); int i
a[i].name="txt"; a[i].value="text"; a[i].isfile=0; i+1
 a[i].name="userfile"; a[i].value="$system$\notepad.exe"; a[i].isfile=1; i+1

Http h.Connect("www.quickmacros.com"); str r
if(!h.PostFormData("form.php" a r "User-agent: QM")) end "failed"
out r
