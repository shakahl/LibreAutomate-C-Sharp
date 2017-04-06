/exe 1
key CSAq ;;show QM window

int w=wait(60 WV win("" "Shell_TrayWnd"))
Acc a.Find(w "PUSHBUTTON" "QTranslate 4.1.0" "class=ToolbarWindow32[]id=1504" 0x1005 60)
#ret

int n d=GetDiskUsage
 int c=GetCPU
rep
	1
	d=GetDiskUsage
	out d
	 out "%-5i %5i" d GetCPU
	if(d<=5)
		n+2
		if(n>=10) break
	else if(n>0) n-1

 BEGIN PROJECT
 main_function  detect when Windows fully booted
 exe_file  $my qm$\detect when Windows fully booted.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {9E1BCC72-4771-4F1B-BC21-5C0B8DDB560D}
 END PROJECT
