 /exe
out
 int w1=child(1001 "List4" "SysListView32" win("Main Window" "BaseWindow_RootWnd"))
int w1=id(2 win("Registry Editor" "RegEdit_RegEdit"))
 int w1=child("" "SysListView32" win("Manage Add-ons" "#32770"))
 int w1=id(1023 win("QM - My Macros - All Programs" "#32770"))

int i n=SendMessage(w1 LVM_GETITEMCOUNT 0 0)
str s
Q &q
for i 0 n
	GetListViewItemText(w1 i s)
	out s
Q &qq
outq
out n

 BEGIN PROJECT
 main_function  test GetListViewItemText
 exe_file  $my qm$\test GetListViewItemText.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {68AC6FA0-7442-44EE-8534-BF29CDBC06EF}
 END PROJECT
