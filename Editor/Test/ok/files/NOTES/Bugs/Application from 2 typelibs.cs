typelib Word {00020905-0000-0000-C000-000000000046} 8.0 0x409 ;;Microsoft Word 8.0 Object Library, ver 8.0

Word.Application a
 Excel.Application ea ;;no error if this is inserted
Excel.Worksheet ws
out ws.Application.UserControl ;;error if after Word.Application
 ExcelSheet es ;;same
