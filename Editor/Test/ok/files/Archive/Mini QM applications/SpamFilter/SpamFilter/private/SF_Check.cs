tray.Modify(2 "QM SpamFilter. Checking ...")

MailBee.Message m
str s su ff account comm
int i n ndel im spam md edit(o.flags&0x100) process

SF_Error ;;clear
if(edit or !hid(__sfmain)) process=1
for(i 0 a.len) a[i].flags|0x4000
if(!o.accounts.len) SF_Error "accounts" "please click Options and specify email account"; goto g1
#if !EXE
if(!o.ff.len) SF_Error "filter functions" "please click Options and create or specify filter function"; goto g1
#endif

foreach account o.accounts ;;for each account
	if(quit) ret
	p.p.ServerName=""
	p.Connect(0x200 account)
	err SF_Error "connect" _error.description p.p.ServerName; continue
	n=p.p.MessageCount; if(!n) goto gcont
	for im 1 n+1 ;;for each message
		if(quit) ret
		spam=0; md=0; comm.all
		su=p.p.GetMessageUID(im)
		if(su.len)
			for(i 0 a.len)
				if(a[i].uid=su) ;;already processed
					if(a[i].flags&0x8000) md=1; comm=a[i].comm ;;marked for delete
					else if(process or a[i].flags&8) goto retrieve ;;process always
					break
			if(i<a.len and !md) a[i].flags~0x4000; continue
		 retrieve
		if(o.flags&2) m=p.p.RetrieveSingleMessageHeaders(im o.nlines)
		else m=p.p.RetrieveSingleMessage(im)
		if(!m) SF_Error "retrieve message" p.p.ErrDesc p.p.ServerName; continue
		
		s=m.GetHeader("X-QMSF")
		if(md) if(s.len) MailHeader m "X-QMSF"; goto g2; else goto g2
		if(s~"restored") goto g3
		
		foreach ff o.ff ;;for each filter
			if(!ff.len) continue
#if !EXE
			spam=call(ff &m &p.p im 0 &comm)
#else
			spam=SF_Filter(&m &p.p im 0 &comm) ;;in exe, all filtering must be in SF_Filter function
#endif
			err SF_Error "filter function" _error.description; continue
			if(quit) ret
			if(spam) break ;;0 unknown, 1 good, -1 spam, -2 spam
		
		sel spam
			case [-1,-2]
			 g2
			if(edit and !md) goto g3
			if(comm.len) MailHeader m "X-QMSF-Comments" comm
			if(spam=-1) p.Save(m); err SF_Error "save message" _error.description; continue
			if(!p.p.DeleteMessage(im)) SF_Error "delete message" p.p.ErrDesc p.p.ServerName; continue
			if(spam=-1) ndel+1
			case else
			 g3
			SFMESSAGE& sfm=a[a.redim(-1)]
			sfm.uid=su; sfm.comm=comm; sfm.size=p.p.Size(im)
			if(spam) sfm.flags|iif(spam=-1 8 4)
			sfm.m=m; m=0
	 gcont
	p.Disconnect
	if(quit) ret

err+ p.Disconnect;; out _error.description
if(quit) ret

 g1
for(i a.len-1 -1 -1) if(a[i].flags&0x4000) a.remove(i)
if(!hid(__sfmain)) SF_Update 0 iif(ndel 0 1)
if(iserr and !but(id(19 __sfmain))) CheckRadioButton __sfmain 18 19 19; SendMessage __sfmain WM_COMMAND 19 id(19 __sfmain)

if(a.len)
	tray.Modify(3 s.format("QM SpamFilter. You have %i new messages." a.len))
	if(__sfmain!win)
		if(o.sound.len) bee o.sound; err
		if(o.flags&17 and win!__sfmain)
			if(o.flags&1) ;;run email app
				 don't run if running. Thunderbird sometimes creates second window.
				_i=ProcessNameToId(o.mailapp)
				if(!_i) run o.mailapp "" "" "" 0x100; err SF_Error "email program" _error.description
			if(o.flags&16)
				if(o.flags&1) wait 10 WA win("" "" o.mailapp); err
				act __sfmain
	ictimer=0; SetTimer __sfmain 10 300 0 ;;blink
else tray.Modify(1 "QM SpamFilter")

err+
