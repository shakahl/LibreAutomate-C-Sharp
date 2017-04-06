out
YouBrowseee

 out
 str html
 IntGetFile(starturl html)
 ;out html; ret
 
 str videofromuser
 str pattern="<a id=''video-from-username-.+? href=''(.*?)''>"
 ARRAY(str) a
 if(!findrx(html pattern 0 4 a)) out "not found"; ret
 int i
 int n1
 n1 = -36
 for(i 0 a.len)
	 str url.from("http://www.youtube.com" a[1 i])
	 out url
	  n1+ 36
	  ;now IntGetFile(url html) or HtmlDoc hd.InitFromWeb(url)
	   <a href="#" onclick="add_friend('nebnoid'); return false;">Als Freund hinzufügen</a>
	   Acc a=acc("YouTube - ." "DOCUMENT" win("YouTube - ." "MozillaUIWindowClass" "firefox.exe" 0x202) "MozillaContentWindowClass" "" 0x1002)
	  int hwnd=win("YouTube - . - Microsoft Internet Explorer" "IEFrame")
	  Htm el=htm("A" "." "" win(". Microsoft Internet Explorer" "IEFrame" "" 0xA02) 0 31 0x22 0 n1)
	  el.Click
	 IntGetFile(url html)
	 run url
	 key (VK_NEXT)
	  WAIT
	 wait 0.5
	 Acc e=acc("Eingeladener Freund  | " "TEXT" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1801 0x40 0x20000040)
	 err
		 goto JUMP
	 goto END
	  JUMP
	 Acc c=acc("Als Freund hinzufügen" "LINK" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1001)
	 err
 
			 goto WAIT
	 wait 0.5
	 c.DoDefaultAction	
	 Acc d=acc("Abonnieren" "PUSHBUTTON" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1001)
	 err
		 goto WAIT
	 wait 0.5
	 d.DoDefaultAction
	  END
	 key+ (VK_CONTROL)
	 key w
	 key- (VK_CONTROL)



