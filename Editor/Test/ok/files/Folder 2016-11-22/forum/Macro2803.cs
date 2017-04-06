out

 capture text elements to array
int wMain=win("" "CabinetWClass")
int w=child("" "DirectUIHWND" wMain 0x0 "accName=Items View") ;;list 'Items View'
WindowText wt.Init(w)
wt.Capture
if(wt.n=0) ret

 count columns
int nCol i n(1) y(wt.a[0].rt.top)
for i 1 wt.n
	WTI& t=wt.a[i]
	out t.rt.top
	if(t.rt.top<=y+6) n+1
	else
		y=t.rt.top
		if(n>nCol) nCol=n
		n=1
out nCol


#ret


out
int i row(-1) col left



ICsv x._create
for i 0 wt.n
	WTI& t=wt.a[i]
	 out t.txt
	if(left=0) row+1; x.AddRowLA(row)
	x.Cell(row col)=t.txt
	
	out t.rt
