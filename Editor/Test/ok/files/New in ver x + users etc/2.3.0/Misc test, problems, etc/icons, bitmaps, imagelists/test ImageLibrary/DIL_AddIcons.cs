 /dlg_image_library
function hDlg

__IImageLibrary- ilib

 ilib.AddIcon("$my qm$\CalcCodeSizeTrayIcon.exe")
 DIL_Load hDlg 1
 ret

ARRAY(str) a; int k kn
int flags=but(14 hDlg)
 DIL_Enum "$My QM$\*.ico" a flags
DIL_Enum "$My QM$\*" a flags|2
 DIL_Enum "$QM$\*.ico" a flags
 DIL_Enum "$QM$\*" a flags|2
 DIL_Enum "$system$\*.exe" a flags
 DIL_Enum "$system$\*.dll" a flags

out
rep val(_s.getwintext(id(20 hDlg)))
	Q &q
	kn=a.len
	 kn=49
	for k 0 kn
		 out a[k]
		ilib.AddIcon(a[k]); err out "FAILED: %s" a[k]
	Q &qq
	outq
DIL_Load hDlg 1
