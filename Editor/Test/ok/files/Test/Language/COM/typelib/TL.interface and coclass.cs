typelib Word {00020905-0000-0000-C000-000000000046} 8.0 0x409 ;;Microsoft Word 8.0 Object Library, ver 8.0
typelib Excel {00020813-0000-0000-C000-000000000046} 1.2 ;;Microsoft Excel 8.0 Object Library, ver 1.2

Word.Application c._create(uuidof(Word.Application))
c.Visible=-1
  out c.Visible
2
c.Quit
1
Excel.Application b._create(uuidof(Excel.Application))
b.Visible=-1; err
 out b.Visible; err
2
