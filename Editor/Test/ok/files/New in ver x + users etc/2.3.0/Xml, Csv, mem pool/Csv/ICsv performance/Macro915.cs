 in QM < 2.3.0.2, this code would be too slow (~20 s)

int i c
str s t

ICsv v=CreateCsv
Q &q
v.FromFile("$desktop$\data.csv")
Q &qq
outq

IStringMap m=CreateStringMap(2)
s.getfile("$desktop$\map.txt")
m.AddList(s " ");;map uses tab as separator

 if(!inp(c "Operate on which column?" "CSV Find & Replace")) ret
c=1

out v.RowCount
int t1=GetTickCount
 for i 0 v.RowCount/8
for i 0 v.RowCount
	t=v.Cell(i c-1)
	lpstr x=m.Get(t)
	if(x) v.Cell(i c-1)=x
	else v.Cell(i c-1)="Not Found"
v.ToFile("$desktop$\output.csv")
int t2=GetTickCount
out t2-t1
bee 2000 200
