 create array for testing
str s1="aaaa 10.123.456.789 bbbb[]aaaa 20.123.456 bbbb[]aaaa 25.123.456.789 bbbb"
ARRAY(str) a
if(findrx(s1 "(10|20|25)(?:\.\d{1,3}){3}" 0 4 a)=0) ret

 int i
 for i 0 a.len
	 out a[0 i]
 ret
 ________________________________

int j k sheetexists(1)
str s2
ARRAY(str) a2 afound

 create sheet name from date
str sheet.time("%x")
sheet.replacerx("[\\/]" "-")

 connect to Excel
ExcelSheet es.Init
Excel.Application app=es.ws.Application

 try to connect to today's sheet
es.ws=app.Worksheets.Item(sheet)
err ;;does not exist. create new
	sheetexists=0
	es.ws=app.Worksheets.Add()
	es.ws.Name=sheet
	
if sheetexists
	 search for each element and add found elements to afound
	es.GetCells(a2 "A:A")
	for j 0 a.len
		for(k 0 a2.len) if(a[0 j]~a2[0 k]) break
		if(k<a2.len)
			afound[afound.redim(-1)]=a[0 j]
			a[0 j].all ;;clear
	
	 message
	if(afound.len)
		for(j 0 afound.len) s2.formata("%s[]" afound[j])
		mes s2 "Found" ""

 add all that did not exist
k=a2.len+1
for j 0 a.len
	if(a[0 j].len) es.SetCell(a[0 j] 1 k)
	k+1
