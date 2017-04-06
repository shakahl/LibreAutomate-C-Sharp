 With Quick Macros you can send and receive email messages.
 You can send html-formatted messages, attach files, manipulate,
 save and forward received messages, use multiple accounts.

 QM email functions use settings of Outlook Express (OE) accounts.
 However, QM cannot retrieve OE passwords, so you have to enter
 them in "QM email settings" dialog. If you don't use OE, create
 email account using the dialog. You can access the dialog through
 one of dialogs in the floating toolbar. Or run this macro.
 OE itself can be installed or not, QM email functions don't use it.

 Email account settings also can be stored in macro. To insert,
 check "Var" in one of dialogs in the floating toolbar. Or
 click "To string" button in Email Account dialog.
 Passwords are encrypted. To use non encrypted password,
 prepend ! to the password (QM 2.3.2).

 QM email functions are based on MailBee COM component:
 <link>http://www.afterlogic.com/</link>,
 <link>http://www.quickmacros.com/com/MailBee.chm</link>.

 To make working with email easier, MailBee classes SMTP
 and POP3 are wrapped into QM classes SmtpMail (send)
 and Pop3Mail (retrieve), and functions SendMail and ReceiveMail.

 Usually, when sending email, it is not necessary to use
 MailBee objects directly. When receiving email, messages are
 retrieved as MailBee.Message objects, so you can easily
 manipulate them (retrieve text, headers, save, forward, etc).
 Also, when retrieving email, often is useful MailBee.POP3 object.

 You can also use MailBee objects directly, but to create valid
 (licensed) MailBee objects, you have to declare QM objects and
 retrieve MailBee object from them.

 Most email functions don't support Unicode. For example, you
 cannot attach a file if there are Unicode characters in name or path.

 If used in exe, need mailbee.dll. Take it from QM folder on your
 computer, and copy to your exe folder on other computers. Don't
 need to register as COM component (QM 2.3.4).

 See also: <SendMail>, <Email samples>, <Pop3Mail.RetrieveMessages>.


MailSetup
