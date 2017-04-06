function $to $subject $text [flags] [$attach] [$cc] [$bcc] [$headers] [`template] ;;flags: 1 html, 2 file, 16 forward, 32 exact.

 Creates or loads/modifies message and adds to internal collection.
 To send all collected messages, use Send.
 See also: <SendMail> function.


MailBee.Message m mm; str s sf s1 s2; lpstr ss; int f mr

 template?
sel template.vt
	case VT_DISPATCH if(template.pdispVal) f|1 ;;Message object
	case VT_BSTR
	if(template.bstrVal.len) ;;eml file
		f|2; s=template; sf.expandpath(s)
		if(FileExists(sf 1)) ;;add all eml files in folder
			sf+iif(sf.end("\") "*.eml" "\*.eml"); Dir d
			foreach(d sf FE_Dir) AddMessage(to subject text flags attach headers d.FullPath)
			ret

if(!empty(text)) f|4
if(!empty(attach)) f|8

m._create

if(f&3) ;;load from template
	if(f&12=0) m.MinimalRebuild=-1; mr=1
	m.CodepageMode=1
	if(f&1) mm=template; m.RawBody=mm.RawBody
	else if(!m.ImportFromFile(sf)) end s.from("cannot load file " sf)

	if(flags&32=0) ;;allow implicit changes
		foreach(s "Message-Id[]Received[]Resent-From[]X-Receiver[]X-UIDL[]X-OriginalArrivalTime") H(m s) ;;remove some headers
		if(flags&16=0) foreach(s "From[]Reply-To[]Return-Path[]Sender[]X-Sender") H(m s) ;;remove sender info. Will add in Send.
	
	if(flags&16) H(m "Cc"); H(m "Bcc") ;;forward
	else if(mr) ;;get cc and bcc from raw headers
		if(empty(cc)) s1=m.GetHeader("Cc"); cc=s1
		if(empty(bcc)) s2=m.GetHeader("Bcc"); bcc=s2
		H(m "Bcc")

 recipients, subject, mailer
if(!empty(to)) H(m "To" to)
if(!empty(cc)) H(m "Cc" cc)
if(!empty(bcc)) m.BCCAddr=bcc ;;!! review this when upgrading MailBee
if(!empty(subject)) H(m "Subject" subject)
if(f&3=0 or flags&32=0) H(m "X-Mailer" s.from("Quick Macros " _qmver_str))

 optional headers
foreach(s headers)
	if(!s.len) continue
	ss=strchr(s ':'); if(ss) ss[0]=0; rep() ss+1; if(ss!=32) break
	if(ss[0]) H(m s ss); else H(m s)

 text
if(f&4)
	if(flags&2) ;;file
		s.expandpath(text)
		if(s.endi(".eml"))
			mm._create
			if(!mm.ImportFromFile(s)) end s.from("cannot load file " s)
			if(f&3) m.BodyText=""
			m.BodyFormat=mm.BodyFormat
			m.BodyText=mm.BodyText; m.AltBodyText=""
		else if(!m.ImportBodyText(s -(flags&1) 0)) end s.from("cannot load file " s)
	else
		if(f&3) m.BodyText=""; m.AltBodyText=""
		m.BodyFormat=flags&1; m.BodyText=text

 MailBee bug: if SSL, and body is empty, and 0 attachments, sometimes adds garbage body text, or sometimes exception. Looks like encrypts some invalid buffer.
BSTR bt=m.BodyText; if(!bt.len) m.BodyText="[]"

 attachments
foreach(s attach) if(s.len) if(!m.AddAttachment(s.expandpath() "" "" "")) end s.from("cannot add attachment " s)

 add to collection
___SMCOLL& c=m_a[]
c.m=m; if(f&2) c.delfile=sf
c.flags=flags&255|(f<<8)

err+ end _error
