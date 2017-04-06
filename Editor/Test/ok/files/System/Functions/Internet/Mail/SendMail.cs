 /
function $to $subject $text [flags] [$attach] [$cc] [$bcc] [$headers] [`template] [$account] ;;flags: 1 html, 2 file, 16 forward, 32 exact, 0x100 dialog, 0x200 events, 0x400 delete, 0x10000 don't send, 0x20000 show source, 0x40000 show message, 0x80000 log.

 Sends email message.
 Error if fails.

 to, cc, bcc - email address.
    Can contain friendly name and comments.
    Can be specified multiple addresses separated by comma.
    Example: "d@e.f, Bill <b@c.d>, Lisa <l@m.n> (Company Name)"
 subject - message subject.
 text - message text.
 flags:
	 These flags are used by SmtpMail.AddMessage:
	  1 text is in html format
	  2 text is file. Can be text file, html file (flag 1 also must be set), or message file (.eml) (flag 1 is ignored).
	  16 template message is for forwarding. This removes CC & BCC headers from template, adds Resent-From header, and does not change From and Reply-To headers.
	  32 don't modify template message (this makes only minimal corrections, if required). By default, some headers (Received, Message-Id, etc) are removed.
	 These flags are used by SmtpMail.Send:
	  0x100 while sending, show progress dialog
	  0x200 process events (when calling from window, etc)
	  0x400 remove sent message(s) from internal collection. If template is file, delete file too.
	  0x800 save sent message(s) to the folder that has been previously set with SetSaveFolder
	 Debugging flags:
	  0x10000 don't send, 0x20000 show source text (bcc is not shown), 0x40000 show real message, 0x80000 log to file "$my qm$\smtp_log.txt"
 attach - list of attachments, eg "$desktop$\file1.txt[]$desktop$\file2.txt"
 headers - list of additional headers, eg "Reply-To: abc@def.gh[]X-Mailer: My App"
 template - email message to use as template. If other arguments are not used (""), it is sent unmodified. Can be either file or object:
	 1. String that specifies message file (.eml). It, for example, can be created and saved in an email program. If template specifies folder, sends all messages (.eml) files from the folder.
	 2. Object of type MailBee.Message. It can be empty or filled. Useful, for example, to forward messages received by Pop3Mail object.
 account - name of an Outlook Express account, or "" or "<default>" to use default account. Also can be multiline string containing account settings.
   OE accounts are listed in "QM Email Settings" dialog that can be accessed from "Send email message" or "Receive email messages" dialog.
   If the account you need is not in the dialog, create new account settings (click New in the dialog ...).
   Before using QM email functions, need to set password in QM email account dialog.
   The password is encrypted. It's possible to use a non-encrypted password: in dialog "Send email message" check "Variable", then in macro replace smtp_password EncryptedPassword with smtp_password !NonEncryptedPassword.

 REMARKS
 Sends directly (uses SMTP), not through email program.
 Gmail and some other SMTP servers require SSL. See examples in QM forum. However it may not work with some security software. If using Avast, disable SSL scanning in Mail Shield settings.

 See also: <Email help>, <Email samples>.


SmtpMail m.AddMessage(to subject text flags attach cc bcc headers template)
m.Send(flags account)
err+ end _error
