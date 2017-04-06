function [flags] [$account] ;;flags: 0x100 dialog, 0x200 events, 0x400 delete, 0x800 save, 0x10000 don't send, 0x20000 show source, 0x40000 show message, 0x80000 log.

 Sends email messages that have been added by AddMessage.
 See also: <SendMail> function.


if(!m_a.len) ret
___EMAILACCOUNT a; str s; int i n mr
int- ___t_smtp_hdlg; if(___t_smtp_hdlg) DestroyWindow(___t_smtp_hdlg); ___t_smtp_hdlg=0

if(flags&0x80000) m_smtp.LogFilePath=s.expandpath("$my qm$\smtp_log.txt"); m_smtp.EnableLogging=TRUE; m_smtp.ClearLog; else m_smtp.EnableLogging=0
m_smtp.EnableEvents=iif(flags&0x300 TRUE 0)
if(flags&0x100)
	lpstr dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 210 66 "QM - sending mail"
 2 Button 0x54030001 0x4 80 48 48 14 "Cancel"
 3 Static 0x54020000 0x4 4 22 32 9 "To:"
 4 Static 0x54020000 0x4 4 34 32 10 "Subject:"
 7 Static 0x54020000 0x4 4 5 124 9 "Connecting..."
 8 Static 0x54020000 0x4 164 5 44 9 ""
 5 Static 0x54020000 0x4 40 22 168 9 ""
 6 Static 0x54020000 0x4 40 34 168 10 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030500 "" "" "" ""
	___t_smtp_hdlg=ShowDialog(dd &sub.Dlg 0 0 1 0 0 &m_smtp)
	m_smtp._setevents("sub." 1)
else m_smtp._setevents

if(!Connect(account a flags&0x10000)) goto ge

n=m_a.len
for i 0 n
	___SMCOLL& c=m_a[i]; if(!c.m) continue
	mr=c.m.MinimalRebuild; if(mr) s=c.m.GetHeader("From"); else s=c.m.FromAddr
	if(!s.len)
		H(c.m "From" iif(a.smtp_displayname.len s.from(a.smtp_displayname " <" a.smtp_email ">") a.smtp_email))
		if(mr) s=c.m.GetHeader("Reply-To"); else s=c.m.ReplyToAddr
		if(!s.len) c.m.ReplyToAddr=a.smtp_replyto
	else if(c.flags&16) H(c.m "Resent-From" a.smtp_email) ;;forward
	
	if(___t_smtp_hdlg)
		s.format("Sending message %i of %i ..." i+1 n); s.setwintext(id(7 ___t_smtp_hdlg))
		s=c.m.ToAddr; s.setwintext(id(5 ___t_smtp_hdlg))
		s=c.m.Subject; s.setwintext(id(6 ___t_smtp_hdlg))
	
	if(flags&0x20000) s=c.m.RawBody; ShowText "QM - SendMail" s ;;Note: currently, Bcc is not displayed
	if(flags&0x40000) s=F"$temp qm$\SendMail ({i}).eml"; if(SaveMessage(c.m s)) run s
	if(flags&0x10000=0)
		m_smtp.Message=c.m; if(!m_smtp.Send) goto ge
		if(flags&0x800) Save(i)
		if(flags&0x400) c.m=0; if(c.delfile.len) del- c.delfile; err

if(flags&0x400) m_a.redim
goto gc
 ge
str s1(m_smtp.ErrDesc) s2(m_smtp.ServerResponse); int e=1
err+ e=2
 gc
m_smtp.Disconnect; if(___t_smtp_hdlg) DestroyWindow(___t_smtp_hdlg); ___t_smtp_hdlg=0
sel e
	case 1 end s.format("%s (0x%X)[][9]Server response: %s" s1 m_smtp.ErrCode s2)
	case 2 end _error


#sub _OnSendProgress
function TotalBytesSent @&Proceed

int- ___t_smtp_hdlg
str s.from(TotalBytesSent/1024 " KB")
s.setwintext(id(8 ___t_smtp_hdlg))


#sub Dlg
function# hDlg message wParam lParam
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDCANCEL
	MailBee.SMTP* smtp=+DT_GetParam(hDlg)
	if(smtp and smtp.Busy) smtp.Abort
ret 1
