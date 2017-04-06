 /exe
 sel(ListDialog("one[]two" "des" "tit" 0x0 0 0 0 5))
	 case 1 out 1
	 case 2 out 2
	 case else ret
 
  deb
 ShowText "ca" "text"

 if(!BrowseForFolder(_s "$qm$" 4 "ttt")) ret
 out _s

 if(!InputBox(_s 0 F"<><a href=''{&sub.Link} ppp''>link</a>" "cap" 0 0 0 0 "" 10)) ret
 out _s
 
 #sub Link
 function# hwnd $params
 mes params


OnScreenDisplay "aaa"
OnScreenDisplay "bbb" 0 0 300 0 0 0xff
 OnScreenDisplay "bbb" 0 0 300 0 0 0xff 2
2
OsdHide
2


 BEGIN PROJECT
 main_function  Macro2316
 exe_file  $my qm$\Macro2316.qmm
 flags  6
 guid  {B87D5607-645F-468D-BF5A-A62ACE5675FC}
 END PROJECT
