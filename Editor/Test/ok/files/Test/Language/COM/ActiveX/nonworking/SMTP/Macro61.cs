typelib MySMTP "C:\Documents and Settings\a\Desktop\New Components\nonworking\SMTP (ocx)\SMTP.ocx"
MySMTP.SMTP c._create
c.ToAddress="gindi@takas.lt"
c.Subject="sub"
c.FromAddress="gindi@takas.lt"
out c.State
c.Send
