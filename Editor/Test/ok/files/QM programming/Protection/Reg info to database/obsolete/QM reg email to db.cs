 Extracts QM registration info from message in OE, and inserts into mysql database.
 Message (reply) must be opened in OE pane.

int i h=win(" Outlook Express")
act h
act GetParent(child("" "Internet Explorer_Server" h 0x1))
key Ca
str s.getsel
ARRAY(str) a
str pat="(?s)^Your registration code:[] ?[](.+?)[].+[][ >]*Order number (.+?) has.+[][ >]*Name .+: (.+?)[].+[][ >]*Email .+: (.+?)[]"
if(findrx(s pat 0 8 a)<0)
	a.create(5)
	if(!inp(a[1] "regcode")) ret
	if(!inp(a[2] "id (eg free-20)")) ret
	if(!inp(a[3] "name")) ret
	if(!inp(a[4] "email")) ret
	
s.format("http://www.quickmacros.com/reg/rcadd.php?orderid=%s&name=%s&email=%s&regcode=%s" a[2].escape(9) a[3].escape(9) a[4].escape(9) a[1].escape(9))
if(mes(s "Add to database?" "YN?")='Y') run s
