function [$to] [$cc] [$bcc] [$subject] [$body] [$files] [flags] ;;flags: 1 send, 4 don't wait.

 Creates new email message in default email program, and optionally sends. Uses MAPI.
 Error if failed.
 Obsolete, use SendMail or run "mailto:...".

 to, cc, bcc - email address. Also can be name from Address Book, or include name and address like "name <address>". Can be list, delimited by semicolons, commas or newlines.
 subject - message subject.
 body - message text.
 files - one or more (multiline list) files to attach.

 REMARKS
 Some programs, eg Eudora, require to enable MAPI (in program's Options dialog).
 Outlook may show Profile dialog. To skip it, set profile name in 'Email message' <macro "TO_EMail">dialog</macro>.
 Does not support Unicode.
 QM 2.4.1: Removed flag 2 - close warning message. It was unreliable because every new email program version may show different message.


ref ___mapi "MAPI"

if(flags&4)
	mac "MailMessage" "" to cc bcc subject body files flags~4
	ret

___mapi.MapiMessage mm
int i nr nf sh e; str sr p
lpstr delim=";,[]"
ARRAY(lpstr) ar
ARRAY(str) af

 body and subject
mm.lpszSubject=subject
mm.lpszNoteText=body

 recipients
sr.from(to "[]" cc "[]" bcc)
bcc=sr+len(to)+len(cc)+4; cc=sr+len(to)+2; to=sr
nr=tok(sr ar -1 delim 1)
if(nr)
	findrx("" "^[^@\s]+@[^@\s]+$" 0 128 p)
	ARRAY(___mapi.MapiRecipDesc) mr.create(nr)
	for(i 0 nr)
		rep() if(ar[i][0]=32) ar[i]+1; else break
		mr[i].ulRecipClass=iif(ar[i]<cc 1 iif(ar[i]<bcc 2 3))
		mr[i].lpszName=ar[i]
		if(!findrx(ar[i] p)) mr[i].lpszAddress=ar[i]
	mm.nRecipCount=nr
	mm.lpRecips=&mr[0]
 files
af=files
if(af.len)
	ARRAY(___mapi.MapiFileDesc) mf.create(af.len)
	for(i 0 af.len)
		str& s2=af[i]
		s2.expandpath(); s2.trim; if(!s2.len) continue
		mf[i].lpszPathName=s2
		mf[i].nPosition=-1
		nf+1
	mm.nFileCount=nf
	mm.lpFiles=&mf[0]

 profile
e=MapiProfile(sh); if(e) goto g1

if(flags&1) flags=0
else flags=___mapi.MAPI_DIALOG

AllowActivateWindows; AllowSetForegroundWindow -1
e=___mapi.MAPISendMail(sh 0 &mm flags 0)
___mapi.MAPILogoff sh 0 0 0
if(!e) ret 1 ;;1 fbc

err+
 g1
if(e==1) ret
int ef; if(!getopt(itemid 1)) ef|4
if(e>=26 or e<1) end ERR_FAILED ef
str se="USER_ABORT,FAILURE,LOGON_FAILURE,DISK_FULL,INSUFFICIENT_MEMORY,ACCESS_DENIED,?,TOO_MANY_SESSIONS,TOO_MANY_FILES,TOO_MANY_RECIPIENTS,ATTACHMENT_NOT_FOUND,ATTACHMENT_OPEN_FAILURE,ATTACHMENT_WRITE_FAILURE,UNKNOWN_RECIPIENT,BAD_RECIPTYPE,NO_MESSAGES,INVALID_MESSAGE,TEXT_TOO_LARGE,INVALID_SESSION,TYPE_NOT_SUPPORTED,AMBIGUOUS_RECIPIENT,MESSAGE_IN_USE,NETWORK_FAILURE,INVALID_EDITFIELDS,INVALID_RECIPS,NOT_SUPPORTED"
tok(se ar 26 "," 1)
end _s.from("MailMessage error: " ar[e-1]) ef
