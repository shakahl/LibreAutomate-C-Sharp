 Functions of ExcelSheet class are used to automate Excel.
 For example, to get or set values of cells.
 A variable of this class represents a worksheet.

 Also you can use all Excel COM functions:
   To connect to Excel, you can use an ExcelSheet variable. Call its Init function.
   The ws member is Excel.Worksheet object. Through it you can access Excel functions. See example.
   More examples: <help>ExcelSheet._Range</help>, <help>ExcelSheet._Book</help>, <help>ExcelSheet._Sheet</help>. Also you can see code of ExcelSheet functions.
   Reference:
     Look in Excel Help -> Visual Basic Reference.
     Or click a function, press F1, click "Google in MS"...
     Or download a chm file with Excel VBA reference from Microsoft. Then F1 will open the file. The required file/path is in QM status bar when you press F1. Tip: disable Script Debugging (Other) in Internet Options.

 NOTES
 Works with Excel 8 (MS Office 97) and later.
 Most functions fail if Excel is in cell edit mode.
 Excel does not allow to undo changes made by macros.
 To save changes, you can use function Save, or Excel Save functions. Other functions don't save.

 Does not work with Office editions that are installed using Click-to-Run technology.
   Then Office is installed in a virtual drive Q, not accessible to Explorer and other applications except Office. QM need to access Excel type library.
   Click-to-Run is used by default with Office 2010 Home/Student and Home/Business, if downloaded directly from Microsoft. It is used with Starter.
   Normal Office installations also are available (except Starter). Log in to where you downloaded Office, find advanced options, download 32-bit or 64-bit, and install.
   You can find more info about Click-to-Run on the web.

 See also: <Database help>

 EXAMPLES
 /exe 1
 create variable and connect to the active Excel worksheet
ExcelSheet es.Init

 get cells A3 and B3
str s
s=es.Cell(1 3)
out s
s=es.Cell(2 3)
out s

 list open workbooks and worksheets. This example also shows the hierarchy of main Excel objects: Application -> Workbook -> Worksheet.
Excel.Application ea=es.ws.Application
Excel.Workbook wb
Excel.Worksheet ws
foreach wb ea.Workbooks
	str wbName=wb.Name
	int nSheets=wb.Worksheets.Count
	out "%i worksheets in %s:" nSheets wbName
	foreach ws wb.Worksheets
		out ws.Name
		 you can assign Excel.Worksheet to ExcelSheet:
		ExcelSheet es3=ws
		out "[9]A1=%s" es3.Cell("A1")

 open My Documents\Book1.xls file in new hidden Excel instance, get worksheet Sheet2, display value of cell B2, append "x", and save
ExcelSheet es2.Init("Sheet2" 8 "$documents$\book1.xls")
str s2
s2=es2.Cell(2 2)
out s2
s2+"x"
es2.SetCell(s2 2 2)
es2.Save
