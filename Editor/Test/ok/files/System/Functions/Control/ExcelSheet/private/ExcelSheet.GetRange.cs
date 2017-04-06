function $range Excel.Range&r [&nr] [&nc] [flags] ;;flags: 1 limit r

 Gets Range object for the specified range.
 Also gets number of rows and columns.
 If whole column or row, limits nc or nr to the used range; if flag 1, also limits r.


r=__Range(range)

if(!&nr && !&nc) if(flags&1) int _nr _nc; &nr=_nr; &nc=_nc; else ret
nr=r.Rows.Count; nc=r.Columns.Count

 limit if whole column or row
if !empty(range)
	POINT+ __ExcelSheet_Limits
	POINT& p=__ExcelSheet_Limits
	if(!p.x) p.x=ws.Columns.Count; p.y=ws.Rows.Count
	
	if nr=p.y or nc=p.x
		Excel.Range ru=ws.UsedRange
		if(nr=p.y) nr=ru.Row-1+ru.Rows.Count
		if(nc=p.x) nc=ru.Column-1+ru.Columns.Count
		if(flags&1) r=r.Resize(nr nc)

err+ end _error
