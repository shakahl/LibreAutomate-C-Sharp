 Many Excel and ExcelSheet functions have a range parameter to specify a cell, row, column or other range of cells.

 Examples of Excel ranges:
   "A1" - cell, "3:3" - row 3, "A:C" - columns A-C,
   "A1:C3" - range consisting of a block of cells,
   "A1:A10,C1:C10,E1:E10" - non-contiguous range (separator depends on your regional settings, eg can be ";"),
   "Named" - named range.

 QM-defined ranges:
   "" or "<used>" - the used range,
   "<all>" - whole sheet,
   "<sel>" or "sel" - selection in the active sheet (can be not in this sheet).
   "<active>" - the active cell (can be not in this sheet).
   All cells of certain type in the used range:
     "<constant>" - cells containing a constant (not formula) value.
     "<number>" - cells containing a constant number or date value.
     "<text>" - cells containing constant text.
     "<formula>" - formula cells.
     "<blank>" - blank cells.

 Examples of using column/row index (can be int variable):
   ExcelRow(3) - "3:3", ExcelColumn(3) - "C:C",
   ExcelRange(1 2) - "A2", ExcelRange(1 3 2 7) - "A3:B7", ExcelRange(0 3 0 7) - "3:7".

 The QM-defined ranges can be used only with ExcelSheet functions. Others - with Excel and ExcelSheet functions.

 The used range is the part of worksheet containing non-empty cells (data, formatting).

 QM 2.3.3. Added all the <> ranges and ExcelX functions.

 <link "http://msdn.microsoft.com/en-us/library/aa139976%28v=office.10%29.aspx">An article about Excel ranges</link>
