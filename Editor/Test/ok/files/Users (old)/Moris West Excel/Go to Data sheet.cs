 typelib Excel {00020813-0000-0000-C000-000000000046} 1.2
 #opt dispatch 1

 Excel.Application app._getactive
 Excel.Worksheet ws=app.Sheets.Item("Sheet3")
 ws.Select

  exception

str vbs=
 set app = GetObject(,"Excel.Application")
 app.Sheets("Sheet3").Select
VbsExec vbs
