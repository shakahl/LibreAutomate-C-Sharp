function'str $xlsFile [$moreParams]

 Creates connection string for Excel.

 xlsFile - Excel workbook file (.xls, .xlsx, .xlsm, .xlsb).
 moreParams - will be appended to the connection string.

 REMARKS
 First row is used for column names.
 Use sheet or range in sql as table name. Examples:
   "Select * from [Sheet1$]"
   "Select * from NamedRange"
   "Select * from [Sheet1$A1:B10]"

 In Excel sheets, don't use mixed types (eg text and numeric) in columns, or some cells will not be retrieved.

 QM 2.3.4. Supports file formats used by Office 2007 and later (.xlsx, .xlsm, .xlsb).


int newFormat=findrx(xlsFile "\.xls[xmb]$" 0 1)>=0

str s sf.expandpath(xlsFile) sp.getpath(sf "")

if(newFormat) s.format("Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};Dbq=%s;DefaultDir=%s;" sf sp)
else s.format("Driver={Microsoft Excel Driver (*.xls)};DriverId=790;Dbq=%s;DefaultDir=%s;" sf sp)
 "FirstRowHasNames=0;" does not work. For text too. For Jet works.

s+moreParams

ret s
