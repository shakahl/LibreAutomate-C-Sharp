 /
function# MailBee.Message&m MailBee.POP3&p msgindex reserved str&comments

 Sample spam filter function.
 Spam filter functions are called for each new email message,
 and must determine whether message is good or spam.
 Return value must be: 1 if message is good, -1 or -2 if message is spam, 0 if not sure.
 If returns 0, message is passed through other filter functions. If no one of
 filter functions returns -1, message is considered good.
 To delete the message without saving, return -2. Don't use p.DeleteMessage.
 
 m is message object
 p is POP3 object that retrieved this message
 msgindex is 1-based index of this message in this server
 reserved currently not used
 comments initially is empty, and can be filled with short string (why this message is spam, etc) that will be displayed in Comments column

 Tips:
 When editing filter function, check "Edit mode".
 To apply changes, save.
 ______________________________________________________________
 
 str s

  subject
 s=m.Subject
 if(find(s "string1" 0 1)>=0) ret 1 ;;good
 if(find(s "string2" 0 1)>=0) ret -1 ;;spam

  sender
 s=m.PureFromAddr
 sel s 1
	 case "friend@z.z": ret 1 ;;good
	 case "spammer@spammers.com": comments="from spammers"; ret -1 ;;spam

  text
 s=m.BodyText; if(m.BodyFormat=1) s=m.GetPlainFromHtml(s)
 if(find(s "Attachment removed: Suspicious file extension")>=0) ret -1

  attachments
 MailBee.Attachment att
 foreach att m.Attachments
	 s=att.Filename
	 sel s 1
		 case "Replaced Infected File.txt" ret -1 ;;spam

  raw header
 s=m.RawHeader
 if(findrx(...)>=0) ...
  etc
  ...
  ...

  real message size
 int size=p.Size(msgindex)
  ...

ret 0 ;;pass through other filters
