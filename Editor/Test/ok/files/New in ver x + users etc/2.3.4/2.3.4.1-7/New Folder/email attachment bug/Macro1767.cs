out
 download messages. This code does not delete them from server. To delete, use flag 2.
ARRAY(MailBee.Message) a
ReceiveMail("" 0 "" 0 a)

 save attachments here
str folderForAttachments.expandpath("$desktop$\folderForAttachments")
mkdir folderForAttachments

int i
for i 0 a.len ;;for each message
	MailBee.Message& m=a[i]
	out m.RawBody
	out m.BodyText
	 out m.Attachments.Count
	MailBee.Attachment t
	foreach t m.Attachments ;;for each attachment
		out t.Filename
		 out t.Content
		if(!t.SaveFile(folderForAttachments t.Filename)) out "Failed to save"