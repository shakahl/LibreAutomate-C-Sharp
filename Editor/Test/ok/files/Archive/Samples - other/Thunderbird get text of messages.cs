 This macro displays text of all email messages in a folder in Thunderbird 3.0.6.

out ;;clear QM output

 find Thunderbird window
int w1=win(" - Mozilla Thunderbird" "MozillaUIWindowClass")
 activate message text pane
act child("" "MozillaWindowClass" w1 0 0 0 7)

 find accessible object of first message
Acc a=acc("" "LISTITEM" w1 "MozillaUIWindowClass" "" 0x1010)
 repeat for each message
rep
	a.DoDefaultAction ;;open
	key Ca ;;select all
	str s.getsel ;;get selected text
	 if(!s.len) 1; key Ca; 1; s.getsel ;;try to enable this if sometimes does not select
	out s ;;show in QM output
	out "---------------------"
	a.Navigate("next"); err break ;;get accessible object of next message
	
