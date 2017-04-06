 /
function# MailBee.Message&m MailBee.POP3&p msgindex reserved str&comm

 ret 1

 Determines whether message m is good or spam.
 Return value must be: 1 if message is good, -1 or -2 if message is spam, 0 if not sure.
 If returns 0, message is passed through other filter functions.
 If no one of filter functions returns -1 or -2, message is considered good.
 To delete the message without saving, return -2. Don't use p.DeleteMessage.


str s from subj body ;;url
from=m.PureFromAddr
subj=m.Subject
body=m.BodyText; if(m.BodyFormat=1) body=m.GetPlainFromHtml(body)
body.trim

 out subj
 out from

sel from 3
	case ["processing@shareit.com","member@paypal.com","info@aruodas.lt"] ret 1
	 case ["forum@quickmacros.com"]
	 sel subj 2
		 case ["Forum post notification - *","New topic notification - *"] ;;,"New Private Message *"
		 findrx(body "^http://.+(?=[])" 0 8 url)
		  out url
		 bee "c:\windows\media\notify.wav"
		  run url "" "" "" 3|0x100; err outb url 1
		 int h=FindTaggedWindow("qm_f" "Mozilla Firefox" "MozillaUIWindowClass")
		 if(h) act h; err
		 else
			  run "firefox" "" "" "" 0x800 win("Mozilla Firefox" "MozillaUIWindowClass" "" 1) h ;;open new firefox window and get its handle
			 run "firefox" "" "" "" 0x800 win("Mozilla Firefox" "MozillaUIWindowClass" "" 1) h ;;open new firefox window and get its handle
			 2
			 TagWindow h "qm_f"
		 run "firefox" url
		 comm="QM Forum"
		 ret -2
		  ret -1
		 
		 case "Fantastico*" comm="fantastico"; ret -1
	case "rcdb@quickmacros.com" comm="rcdb"; ret -1
	
	case "accounting@shareit.com" if(find(body "Your account did not have any activity")>=0) ret -1
	
	case "core.desk@surpasshosting.com" if(find(subj "Invoice" 0 1)>=0) ret -1
	case "alerts@cnet.online.com" ret -1
	case "newsletters@cnet.online.com" ret -1
	case "phpbbspprt840@gmail.com" ret -1
	case "*@sdg.lt" ret -2
	case "*@sharewareonsale.com" ret -2
	case "password@twitter.com" comm="twitter login"; ret -1
	
	 case ["noreply@mysubscribe.org","lois.barry@att.net","saudi_com@yahoo.com"] ret -2

 sel from 3
	 case ["*@facebookmail.com"] ret -1
sel subj 2
	case "Help your friends recognize you" ret -2
	case "LINK BACK NEEDED:*" ret -2

if(findrx(subj "(macro|macros|qm)" 0 1)>=0) ret 1
 if(findrx(body "(macro|macros|qm|quickmacros?)" 0 3)>=0) ret 1 ;;some spam use quickmacros.com in body
if(findrx(body "(macro|macros|qm?)" 0 3)>=0) ret 1

if(subj.beg("***SPAM***")) comm="SPAM"; ret -1

if(find(body "----- Original Message -----")>=0) ret 1

 if(findrx(subj "Bacheelor|degree" 0 1)>=0) comm="spam"; ret -1
 if(find(body "nice girl" 0 1)>=0) comm="nice girl"; ret -1
 if(find(body "VIRUS ALERT")>=0) comm="VIRUS ALERT"; ret -1

 if(findrx(subj "^(Re|Fwd)(\[\d+\])?: ?$" 0 1)=0) comm="spam ?"; ret -1

from=m.FromFriendlyName
sel from 1
	case "Mail Delivery System" ret -1
	case "System Administrator" comm="System Administrator"; ret -1
	 case ["bill ritchie","ukcasanova74"] ret -2

 if(m.Attachments.Count) comm="attachment"; ret -1
 MailBee.Attachment att
 foreach att m.Attachments ;;somehow this makes MailBee typelib have refcount 1 in dtor after releasing
	 comm="attachment"; ret -1
	 s=att.Filename
	 sel s 3
		 case ["Replaced Infected File.txt","Replaced Blocked File.txt","warning.htm"]
		 comm="replaced virus"; ret -1
		 case "*.gif"
		 if(body.len<30) comm="spam (gif)"; ret -1

if(find(body "Attachment removed: Suspicious file extension")>=0) comm="replaced virus"; ret -1
if(find(body "Replaced Infected File.txt")>=0) comm="removed virus"; ret -1
if(find(body "Inbound message INFECTED")>=0) comm="Avast: removed virus"; ret -1

 this is too unreliable, filters some good messages
 body.formata(" %s" subj)
 int score minscore=5 ;;change this
 body.replacerx("[^[:alnum:]]+" "")
 lpstr spamwords="valium|viagr|ciali|medications|penis|rolex|Bacheelor|sex"
 if(findrx(body spamwords 0 1|16)>=0) score=minscore
 else
	  spamwords="advert|alert|announce|breakout|business|campaign|company|expected|forecast|hot|investor|market|news|offer|opportunit|price|potent|product|profit|promot|provid|recent|strong|super|week"
	 spamwords="alert|investor|price|vitamins|stock|st0ck|winner|powerline|big sales"
	 ARRAY(str) a
	 score=findrx(body spamwords 0 1|4|16 a)
 if(score>=minscore) comm.format("spam (score=%i)" score); ret -1

s=m.RawBody
if(find(s "Windows-1251" 0 1)>=0 or find(s "koi8-r" 0 1)>=0) comm="russian spam"; ret -1
if(find(s "windows-1256" 0 1)>=0) comm="arabian spam"; ret -1
if(find(s "GB2312" 0 1)>=0) comm="china spam"; ret -1
 if(findrx(s "(?s)<body[^>]*>\s*(<[^>]+>\s*)*<img " 0 1)>0) comm="img"; ret -1
 if(find(s "<IMG alt=3D''''" 0 1)>0) comm="img2"; ret -1

 if(find(s "*TEST CRASH*")>=0) ret -1

ret 0 ;;pass through other filters
