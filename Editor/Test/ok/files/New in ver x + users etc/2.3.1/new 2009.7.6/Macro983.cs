Dir d
if d.dir("q:\app\debug\qm.pdb" 0) ;;if exists
	FILETIME tm tc
	d.TimeModified(tm 0 0 1)
	d.TimeCreated(tc 0 0 1)
	 out tm.dwLowDateTime
	
	__HFile f.Create("q:\app\qm.exe" OPEN_EXISTING FILE_WRITE_ATTRIBUTES)
	out SetFileTime(f &tc 0 &tm)

 BEGIN PROJECT
 main_function  Macro983
 exe_file  $desktop$\Macro983.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {90851DF8-0903-44A0-96A4-D8E8F83ABC1A}
 END PROJECT
