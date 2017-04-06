 this example clicks a link that displays a message box "VBScript", and closes the message box
int w1=win("Internet Explorer" "IEFrame")
act w1
Htm el=htm("A" "msgbox" "" w1 0 0 0x21)
el.ClickAsync
int w2=wait(10 WV "VBScript")
 int w2=wait(10 WV win("VBScript" "#32770" w1 32)) ;;waits for window that is owned by w1
clo w2
