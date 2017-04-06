MailBee.Message m._create
MailBee.Attachment att
 MailBee.IAttachments c=m.Attachments
 foreach att c
foreach att m.Attachments
	out 1

 m=0
 outref c

 c.AddRef



 Wsh.FileSystemObject fso._create
 Wsh.Drive dr
  Wsh.Drives ds=fso.Drives
  foreach dr ds
 foreach dr fso.Drives
	 out dr.DriveLetter

 ExcelSheet es.Init
 Excel.Comment c
 Excel.Comments cc=es.ws.Comments
  foreach c cc
 foreach c es.ws.Comments
	 out c.Author

 ExcelSheet es.Init("" 8|16|0x100)

 Speak "text" 0 "voice"

 ExcelSheet es.Init("Sheet3" 1)
