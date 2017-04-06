/exe

type DEX_DATA2 __IImageLibrary'il
DEX_DATA2 d

d.il=__CreateImageLibrary
d.il.Load(":10 $my qm$\dex_imagelib.xml")
#if !EXE
err
	out "creating new"
	d.il.CreateNew(16 ":10 $my qm$\dex_imagelib.xml")
	str icons=
	 text.ico
	 keyboard.ico
	 mouse.ico
	 lightning.ico
	 folder.ico
	 cut.ico
	 wait.ico
	 shell32.dll,6
	 shell32.dll,7
	 shell32.dll,8
	foreach(_s icons) d.il.AddIcon(_s)
	d.il.Save(":10 $my qm$\dex_imagelib.xml")
#endif

if(!ShowDialog("DEX_Dialog2" &DEX_Dialog2 0 0 0 0 0 &d)) ret

 BEGIN PROJECT
 main_function  DEX_Main2
 exe_file  $my qm$\DEX_Main2.exe
 icon  $qm$\function.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  22
 end_hotkey  0
 guid  {AD44AFFE-6D06-48E1-9062-081FE351DDF6}
 END PROJECT
