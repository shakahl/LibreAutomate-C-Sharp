 /exe 1

ifa "PowerPoint Slide Show" ;;in slideshow (fullscreen)
	 this code is for 3)
	int i=PowerPointSlidesGetCurrent(1)
	err i=PowerPointSlidesGetCount
	key Z ;;press Esc to stop slideshow
	PowerPointSlidesSetCurrent i
else ;;in editor
	 this code is for 1) and 5)
	key SF5 ;;press Shift+F5 to start show from current slide

 BEGIN PROJECT
 main_function  Macro1415
 exe_file  $my qm$\Macro1415.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {5051CB30-C55E-4A0D-B1A9-AA9281038F52}
 END PROJECT
