 I just upgraded to IE7 and a macro that I had written fails when it tries to launch IE. IE hangs and I have to terminate the task. Below is an excerpt from the macro.

int GrayHair
web "https://mailreport.grayhairsoftware.com" 0x9 0 "" 0 GrayHair
wait 0 I
'xxxxxxxxxxxxxxxxxxxxxxx ;;(hidden for security reasons)
wait -1 "MailReport - Windows Internet Explorer"
wait 3
int w1=act(win("MailReport - Windows Internet Explorer" "IEFrame"))

 ------------------------

 Possibly it is the bug (fixed) where, when using flags 9, IE is deactivated.
 Then wait would wait forever.
