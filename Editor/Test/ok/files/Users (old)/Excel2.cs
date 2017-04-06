 PART 1

 get lookup values
ExcelSheet es.Init("exclusives")
ARRAY(str) a
es.GetCells(a "B2:B4")

int i
 for(i 0 a.len) out a[0 i]

 enum sheets
Excel.Worksheet ws
foreach ws es.ws.Application.Worksheets
	str name=ws.Name
	if(name~"exclusives") continue
	 search and replace
	Excel.Range ru ra f
	ru=ws.UsedRange
	ra=ws.Cells
	for i 0 a.len
		 r.Replace(a[0 i] "Brand") ;;this would be easier and probably faster, but shows message box if not found
		
		f=ru.Find(a[0 i] @ @ @ @ 1)
		rep
			if(!f) break
			f.Value="Brand"
			f=ru.FindNext(f)
		
	 PART 2
	
	 find "Brand" cells
	int j col row col0 row0
	f=ru.Find("Brand" @ @ @ @ 1)
	for j 0 1000000000
		if(!f) break
		col=f.Column; row=f.Row
		if(!j) col0=col; row0=row ;;save first cell position
		else if(col=col0 and row=row0) break ;;stop searching because returned to the beginning
		 out "%c%i" '@'+col row
		
		 clear next 8 cells in the row
		str s.format("%c%i:%c%i" 'A'+col row 'A'+col+7 row)
		ra.Range(s).Clear
		
		f=ru.FindNext(f)
		
		
		