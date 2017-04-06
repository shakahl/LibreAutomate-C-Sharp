 \
function [$text]

 Sends text to clipboards of all computers listed in the NS_Setup dialog.
 If text is not used, sends currently selected text.


OnScreenDisplay "Sending text to other computer(s)..." -1 0 0 0 0 0 8

str s
if(getopt(nargs)) s=text
else s.getsel; if(!s.len) ret

ARRAY(str) a; int i
RegGetValues a "\NetShare" 0 1
if(!a.len)
	mes "At first you need to specify computers. Then retry."
	mac "NS_Setup"
	ret

#compile __CNetworkShare
CNetworkShare ns
for(i 0 a.len)
	ns.Init(a[0 i] a[1 i])
	ns.SendText(s)
