IDispatch oolApp._create("Outlook.Application")
IDispatch email = oolApp.CreateItem(0)
email.Recipients.Add("abcaashn@gmail.com")

 Create the body of the email
str MailBody =
 <!DOCTYPE HTML PUBLIC "-//W3C//DTD W3 HTML//EN"><HTML>
 <HEAD><TITLE>No Invoices</TITLE></HEAD>
 <BODY>
 <B>For Your Information</B>,<BR><BR>
 This is Sample Email.<BR><BR>
 </BODY></HTML>

 Send the Email
email.Subject = "No Invoices Issued"
email.HTMLBody = MailBody
email.Send
