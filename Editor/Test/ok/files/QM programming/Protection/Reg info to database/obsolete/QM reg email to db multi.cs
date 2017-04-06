 Extracts QM registration info from multiple messages in OE, and pastes in notepad (regcodes.txt) to be imported into mysql database. To import this file, use myphpadmin in cpanel.
 Before executing, open 'Reply' folder in OE, and select first message to process.

int nemails=1 ;;number of messages to process
str s email ss pat
ARRAY(str) a
rep nemails
	int w1=act(win(" Outlook Express" "Outlook Express Browser Class"))
	'Y
	int w2=wait(5 win("" "ATH_Note"))
	lef 122 102 w2
	s.getsel; if(findrx(s "(?<=<).+?(?=>)" 0 0 email)<0) email.all
	lef 93 159 w2
	'Ca
	s.getsel
	pat="^(?s)Your registration code:[] [](.+?)[].+[]Order number (.+?) has.+[]Name +: (.+?)[]"
	if(findrx(s pat 0 9 a)<0 or !email.len)
		if(mes("continue?" "failed" "YN")='Y'); goto g1; else break
	ss.formata("%s;%s;%s;%s[]" a[2] a[3] email a[1])
	 g1
	clo w2
	'D
run "notepad"; 1; act id(15 "Notepad")
ss.setsel
