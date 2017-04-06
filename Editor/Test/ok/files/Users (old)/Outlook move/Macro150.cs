str foldername="TestFolder"

typelib Outlook {00062FFF-0000-0000-C000-000000000046} 9.2
Outlook.Application app._getactive

 get selected message
Outlook.MailItem msg=app.ActiveExplorer.Selection.Item(1); err out "no selection"; ret
 out msg.Subject

 get folder
Outlook.MAPIFolder froot=app.GetNamespace("MAPI").Folders.Item(1)
Outlook.MAPIFolder fdest=froot.Folders.Item(foldername)

 move
msg.Move(fdest)
