function MailBee.Message&message [str&file]

 Saves message.

 message - message object.
 file - variable that receives full path of the file.

 REMARKS
 Makes unique filename from subject.
 Folder should be previously set with SetSaveFolder.


if(!m_folder.len) SetSaveFolder("$my qm$\Pop3Mail"); err end _error
str ss sss
str& s=iif(&file &file &sss)

ss=message.Subject; if(!ss.len) ss="Message"
ss.fix(50 2); ss.replacerx("[\\\/\:\*\?\''\>\<\|\x7f-\xff\x1-\x1f]" "_")
s.from(m_folder "\" ss ".eml")
s.UniqueFileName

if(!MailSaveMessage(message s)) end _error
