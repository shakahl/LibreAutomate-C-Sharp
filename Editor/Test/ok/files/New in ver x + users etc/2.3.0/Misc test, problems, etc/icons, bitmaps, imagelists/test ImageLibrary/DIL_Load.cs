 /dlg_image_library
function hDlg [reload]

int h=id(3 hDlg)

__IImageLibrary- ilib
str xf="$desktop$\image library.xml"
if(reload)
	SendMessage h LVM_SETITEMCOUNT 0 0
	ilib.Save(xf)
	 ilib.ToString(xf); out xf
else
	 xf.getfile(xf)
	int t1=perf
	ilib.Load(xf)
	err
		out "failed to load. %s" _error.description
		ilib.CreateNew
	int t2=perf
	out t2-t1
	
	int es=LVS_EX_FULLROWSELECT|LVS_EX_INFOTIP
	SendMessage h LVM_SETEXTENDEDLISTVIEWSTYLE es es
	TO_LvAddCol h 0 "File" 350

SendMessage h LVM_SETIMAGELIST LVSIL_SMALL ilib.Himagelist
SendMessage h LVM_SETITEMCOUNT ilib.Count 0
