str foldername="TestFolder"

IDispatch app._getactive("Outlook.Application")

  get selected message
IDispatch msg=app.ActiveExplorer.Selection.Item(1); err out "no selection"; ret
 out msg.Subject

  get folder
IDispatch froot=app.GetNamespace("MAPI").Folders.Item(1)
IDispatch fdest=froot.Folders.Item(foldername)

 move
msg.Move(fdest)
