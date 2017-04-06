 /exe

 change these
str workbookFile ;;if this variable empty, will use workbook currently open/active in Excel 
 str workbookFile="Q:\Downloads\ExcelGanttChart.xls"
str folder="$desktop$\charts" ;;will save here, in a subfolder of workbook name. Will create folders.
int openFolder=1 ;;1 or 0

 _____________________________________

 connect to Excel or open workbookFile
ExcelSheet es
if(!workbookFile.len) es.Init; else es.Init("" 8 workbookFile)
 variables
ARRAY(Excel.Chart) a
Excel.Chart c
int i
 get chart sheets
Excel.Workbook b=es._Book
foreach(c b.Charts) a[]=c
 gets chart objects of other sheets
Excel.Worksheet w
foreach w b.Worksheets
	for(i 1 w.ChartObjects.Count+1) a[]=w.ChartObjects(i).Chart
 save
str wbName=b.Name
out F"{a.len} charts found in {wbName}"
if(!a.len) ret
folder.expandpath(F"{folder}\{wbName}")
if(!dir(folder 1)) mkdir folder; else del- F"{folder}\*.jpg"
for i 0 a.len
	c=a[i]
	str name=c.Name
	out name
	str file=F"{folder}\{name}.jpg"; file.UniqueFileName
	c.Export(file)
b.Saved=TRUE
 see what we have
if(openFolder) run folder

 BEGIN PROJECT
 
 END PROJECT
