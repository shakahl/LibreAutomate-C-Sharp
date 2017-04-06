 act "Outlook"
 0.5

typelib Outlook {00062FFF-0000-0000-C000-000000000046} 9.2 
 #opt dispatch 1
Outlook.Application a._getactive 
Outlook.Inspector i = a.ActiveInspector 
Outlook.MailItem n = i.CurrentItem 
Outlook.Attachments p = n.Attachments 

p.Item(1).SaveAsFile("c:\taimport.txt")

 typelib Outlook {00062FFF-0000-0000-C000-000000000046} 9.2
 Outlook.Application a._getactive
 Outlook.Inspector i = a.ActiveInspector
 Outlook.MailItem n = i.CurrentItem
 Outlook.Attachments p = n.Attachments
 p.Item(1).SaveAsFile("c:\taimport.txt")

