 \tubeadder
 \Dialog_Editor

spe -1

AddTrayIcon "Youtube.ico" "TubeADDER"

str controls = "3"
str e3url
e3url="http://www.youtube.com/results?search_query=handy+tricks&aq=f"
if(!ShowDialog("YouBrowseee" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 347 162 "Copy and paste Youtube Link here:"
 1 Button 0x54030001 0x4 2 20 228 21 "OK"
 2 Button 0x54030000 0x4 246 20 98 21 "Cancel"
 3 Edit 0x54030080 0x200 0 2 344 16 "url"
 5 Static 0x54000000 0x0 160 62 178 92 "This will only work with Mozilla Firefox 3.5.11![]Other versions may work too, but im not sure bout that..[][]THIS PROGRAM IS FOR INTERNAL USE ONLY! DO NOT DISTRIBUTE, REDISTRIBUTE OR SHARE IT IN ANY KIND! [][]This Program is only for members of ZAWITSCHA![]For questions, errors or anything else contact me at dan@clan-wares.de"
 6 Static 0x54000000 0x0 8 62 140 92 "Open your Browser and login to your YouTube account.[][]Search for videos.[][]Copy and paste the Url.[][]Click the OK Button."
 4 Button 0x54020007 0x0 2 50 342 110 "Important Information"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "" "" ""

 str starturl
out
str html
IntGetFile e3url html
;out html; ret

str videofromuser
str pattern="<a id=''video-from-username-.+? href=''(.*?)''>"
ARRAY(str) a
if(!findrx(html pattern 0 4 a)) out "not found"; ret
int i
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
	run url
	key (VK_NEXT) ;;?
	 WAIT
	wait 0.5
	Acc e=acc("Eingeladener Freund  | " "TEXT" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1801 0x40 0x20000040)
	 Acc e=acc("Invited Friend  | " "TEXT" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1801 0x40 0x20000040)
	err
		out "JUMP"
		goto JUMP
	out "Bereits eingeladen"
	goto END
	 JUMP
	Acc c=acc("Als Freund hinzufügen" "LINK" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1001)
	 Acc c=acc("Add as Friend" "LINK" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1001)
	err
		out "WAIT"
		goto WAIT
	wait 0.5
	out "Freund eingeladen"
	c.DoDefaultAction	
	Acc d=acc("Abonnieren" "PUSHBUTTON" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1001)
	 Acc d=acc("Subscribe" "PUSHBUTTON" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1001)
	err
		out "Bereits Abonniert"
		goto END
	wait 0.5
	out "Abo angenommen"
	d.DoDefaultAction
	 END
	wait 0.2
	key+ (VK_CONTROL)
	wait 0.2
	key w
	wait 0.2
	key- (VK_CONTROL)
	wait 0.2

