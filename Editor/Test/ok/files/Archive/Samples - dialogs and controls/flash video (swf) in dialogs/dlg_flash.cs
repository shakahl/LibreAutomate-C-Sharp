\Dialog_Editor

 This example uses web browser control.

lpstr html=
 <HTML><HEAD>
 <SCRIPT src="res://mshtml.dll/objectembed_neutral.js"></SCRIPT>
 </HEAD>
 <BODY leftMargin=0 topMargin=0 scroll=no onload=ObjectLoad(); objectSource="file:///C:/Users/G/Desktop/digitalclock.swf"><EMBED src=file:///C:/Users/G/Desktop/digitalclock.swf width="100%" height="100%" type=application/x-shockwave-flash fullscreen="yes"></BODY></HTML>

str controls = "3"
str ax3SHD
ax3SHD=html
if(!ShowDialog("" 0 &controls)) ret


 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 141 50 "Dialog"
 3 ActiveX 0x54030000 0x0 18 12 104 24 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "" ""
