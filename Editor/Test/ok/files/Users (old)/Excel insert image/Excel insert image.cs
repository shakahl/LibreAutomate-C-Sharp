ExcelSheet es.Init
IDispatch ws=es.ws ;;use IDispatch because then is easier to make Excel macro work in QM

ws.Range("B2").Select
Excel.Picture p=ws.Pictures.Insert("c:\windows\soap bubbles.bmp")
ws.Rows("2:2").RowHeight = p.Height
ws.Columns("B:B").ColumnWidth = p.Width/5.3 ;;p.Width is in pixels whereas ColumnWidth is characters...

es.ws.Application.Run("Macro1")
