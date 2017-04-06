ExcelSheet es.Init
IDispatch d=es.ws
d.Range("B2").Select
d.Rows("2:2").RowHeight = 165
d.Columns("B:B").ColumnWidth = 64
d.Pictures.Insert("C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Sunset.jpg").Select
