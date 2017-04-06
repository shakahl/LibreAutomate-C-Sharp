function [index]

 Saves email messages that have been added by AddMessage.

 index - 0-based message index. If used and >=0, saves single message. Else saves all.

 REMARKS
 Makes unique filename from subject.
 Folder should be previously set with SetSaveFolder.


if(!m_folder.len) SetSaveFolder("$my qm$\SmtpMail"); err end _error
str s ss
if(getopt(nargs) and index>=0) goto g1

for index 0 m_a.len
	 g1
	___SMCOLL& c=m_a[index]; err end _error
	if(!c.m) continue
	ss=c.m.Subject; if(!ss.len) ss="Message"
	if(ss.len) ss.fix(50 2); ss.replacerx("[\\\/\:\*\?\''\>\<\|\x7f-\xff\x1-\x1f]" "_")
	s.from(m_folder "\" ss ".eml")
	s.UniqueFileName
	if(!SaveMessage(c.m s)) end F"{ERR_SAVE} {s}"
