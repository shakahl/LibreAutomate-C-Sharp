ExcelSheet es.Init
Excel.Worksheet ws=es.ws
ws.Range("B2").Select
Excel.Pictures pc=ws.Pictures
Excel.Picture p=pc.Insert("C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Sunset.jpg")
ws.Rows("2:2").RowHeight = p.Height
d.Columns("B:B").ColumnWidth = p.Width
