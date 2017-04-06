out
str eml="$desktop$\email.eml"
 str eml="$desktop$\email - copy.eml"
eml.expandpath

 save attachments here
str folderForAttachments.expandpath("$desktop$\folderForAttachments")
mkdir folderForAttachments

MailBee.Message m._create
 MailBee.Message m2._create
 m2.RawBody=m.RawBody

m.CodepageMode=1
 m.Codepage=65001

m.ImportFromFile(eml)
 out m.Attachments.Count
MailBee.Attachment t
foreach t m.Attachments ;;for each attachment
	out t.Filename
	
	if(!t.SaveFile(folderForAttachments t.Filename)) out "Failed to save"
