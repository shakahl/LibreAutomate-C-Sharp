 Functions of SmtpMail class can be used to send multiple email messages.
 To send single message, you can use the SendMail function instead.
 See also: <Email help>, <Email samples>, <SendMail>.

 EXAMPLE
SmtpMail mail
mail.AddMessage("a@test.com" "subject1" "text1")
mail.AddMessage("b@test.com" "subject2" "text2")
mail.Send(0x100)
