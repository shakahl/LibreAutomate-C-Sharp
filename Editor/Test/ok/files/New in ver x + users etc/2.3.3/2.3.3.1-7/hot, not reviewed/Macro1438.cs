 /exe 1
ExcelSheet es.Init
str s s2
int i nRows
nRows=es.NumRows
for i 1 nRows+1
	 get cell A
	es.GetCell(s 1 i)
	out s
	
	 set cell C
	s2="NO"
	es.SetCell(s2 3 i)

 BEGIN PROJECT
 main_function  Macro1438
 exe_file  $my qm$\Macro1438.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {E0F5E658-2997-497B-8020-0AAE882374DD}
 END PROJECT
