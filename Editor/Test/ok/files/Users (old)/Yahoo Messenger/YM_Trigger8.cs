 This function watches for new instant messages in Yahoo Messenger, and calls YM_NewMessage for each new message.
 Starts when "Instant Message" window is opened, and exits when the window is closed.
 To always start manually, disable trigger. To stop manually, use the Threads dialog.
 All this can work in the background (no mouse, no keys).
 Works with Yahoo Messenger 8.

 Possible problems:
 May not work with other versions of Yahoo Messenger.
 Possibly, you will have to edit the trigger (if window name does not end with "Instant Message").
 Gets only single line of message text. If a subsequent line matches "text: text" pattern, interprets it as new message.


MSHTML.IHTMLElement el
int i j n him nm
str text prevtext name msg
ARRAY(CHARRANGE) a

if(getopt(nthreads)>1) ret

him=val(_command)
if(!him)
	him=win("Instant Message" "YSearchMenuWndClass")
	if(!him) end "''Instant Message'' window must be opened" 4

rep
	0.5
	if(!IsWindow(him)) ret
	if(!el) el=htm("HTML" "" "" him 0 0 32 10)
	
	text=el.innerText; text.rtrim
	if(text=prevtext) continue
	nm=text.beg(prevtext)
	i=prevtext.len
	prevtext=text
	if(!nm) continue

	n=findrx(text "^(.+?): (.+?)$" i 12 a)
	if(n<1) continue
	for(j 0 n)
		name.get(text a[1 j].cpMin (a[1 j].cpMax-a[1 j].cpMin))
		msg.get(text a[2 j].cpMin (a[2 j].cpMax-a[2 j].cpMin))
		YM_NewMessage name msg him
		0.2
