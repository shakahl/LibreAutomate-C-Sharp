 /
function# $account flags $saveFolder [ARRAY(str)&savedFiles] [ARRAY(MailBee.Message)&messages] ;;flags: 1 only headers, 2 delete from server, 0x100 dialog, 0x200 process events, 0x80000 log, 0x1000000 delete old saved messages (eml files)

 Downloads email messages from POP3 server.
 Returns the number of messages.
 Error if fails.

 account - name of an Outlook Express account, or "" or "<default>" to use default account. Also can be multiline string containing account settings.
   OE accounts are listed in "QM Email Settings" dialog that can be accessed from "Send email message" or "Receive email messages" dialog.
   If the account you need is not in the dialog, create new account settings (click New in the dialog ...).
   Before using QM email functions, need to set password in QM email account dialog.
   The password is encrypted. It's possible to use a non-encrypted password: in dialog "Receive email messages" check "Variable", then in macro replace pop_password EncryptedPassword with pop_password !NonEncryptedPassword.
 flags:
   0x1000000 - before saving (if used saveFolder), delete existing messages (.eml files) from the folder.
   Other flags are the same as with <help>Pop3Mail.RetrieveMessages</help>.
 saveFolder - folder where to save messages. If "", messages will not be saved (instead you can use the messages argument to get messages in memory).
 savedFiles - receives paths of saved messages (eml files). Use 0 if not needed.
 messages - receives message objects in memory. Use 0 if not needed.

 See also: <Pop3Mail.RetrieveMessages>

 EXAMPLES
  downloads messages using default account, saves to "pop" folder on desktop, and deletes messages from server
 ARRAY(str) a; int i nmessages
 nmessages=ReceiveMail("" 2|0x100 "$desktop$\pop" a)
 out nmessages
 for i 0 a.len
	 out a[i]

  downloads messages using default account and displays subject and email address; does not delete messages from server
 ARRAY(MailBee.Message) am; int i; str subj from
 ReceiveMail("" 0 "" 0 am)
 for i 0 am.len
	 subj=am[i].Subject
	 from=am[i].FromAddr
	 out "%s, from %s" subj from


Pop3Mail p; MailBee.Message m
int n save=len(saveFolder); str sf

if(&savedFiles) savedFiles.redim
if(&messages) messages.redim

if(save) p.SetSaveFolder(saveFolder flags&0x1000000!=0)

foreach m p.RetrieveMessages(flags&0xffffff 0 account)
	if(save) p.Save(m sf)
	if(&savedFiles) savedFiles[]=sf
	if(&messages) messages[]=m
	n+1

ret n
err+ end _error
