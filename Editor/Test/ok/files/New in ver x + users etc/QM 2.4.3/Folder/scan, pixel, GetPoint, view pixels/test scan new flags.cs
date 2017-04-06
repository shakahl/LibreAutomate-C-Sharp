 RECT r
 SetRect &r 50 50 60 100 ;;coordinates must be relative to the main search area - screen, window/control, window/control client area, or accessible object
 int w=win("Registry Editor" "RegEdit_RegEdit")
 scan "image:h4E960766[]image:hBA9A9A88" id(2 w) 0 1|2|16|0x800 ;;list

 int w=win("Registry Editor" "RegEdit_RegEdit")
 wait 0 -S "image:h64B66B6D" id(2 w) 0 16|0 ;;list
 int w=win("Registry Editor" "RegEdit_RegEdit")
 wait 0 -S "" id(2 w) 0 16|0x400 ;;list

 int w=win("Registry Editor" "RegEdit_RegEdit")
 Acc a.Find(w "LISTITEM" "CoolSwitchColumns" "class=SysListView32[]id=2" 0x1005)
 wait 0 S "image:h73ED40B8" a 0 1|16|0x400 ;;list

 int w=win("Registry Editor" "RegEdit_RegEdit")
 wait 0 S "image:h73ED40B8" id(2 w) 0 1|16|0x400 ;;list
 int w=win("EM_SETCUEBANNER Message - Microsoft Windows SDK - Microsoft Document Explorer" "wndclass_desked_gsk")
 scan "Macro2536.bmp" child("" "Internet Explorer_Server" w 0x0 "accName=ms-help://MS.W7SDK.1033/MS.W7SDKCOM.1033/Controls/controls/editcontrols/editcontrolreference/editcontrolmessages/em_setcuebanner.htm") 0 1|2|16 ;; 'ms-help://MS.W7SDK.1033/MS....'

 ARRAY(RECT) a
 int w=win("Registry Editor" "RegEdit_RegEdit")
 if(scan("image:h56B8277A[]image:h64B66B6D" id(2 w) 0 16 0 a)) ;;list
	  sample code, shows how to use the array
	 int i
	 for i 0 a.len
		 RECT& rr=a[i]
		 mou rr.left rr.top
		 0.5

 int w=win("Firefox" "MozillaWindowClass")
 act w
 wait 10 -S "image:h787BB9A8" w 0 16

in w=win("Mozilla Firefox Start Page - Mozilla Firefox" "MozillaWindowClass")
act w
wait 0 -S "" w 0 16|0x1100
