int i c
str s t

ICsv v=CreateCsv
str s1.getfile("$desktop$\data.csv")

IStringMap m=CreateStringMap(2)
s.getfile("$desktop$\map.txt")
m.AddList(s " ");;map uses tab as separator

 if(!inp(c "Operate on which column?" "CSV Find & Replace")) ret
c=1

for i 0 v.RowCount
	t=v.Cell(i c-1)
	lpstr x=m.Get(t)
	if(x) v.Cell(i c-1)=x
	else v.Cell(i c-1)="Not Found"
v.ToFile("$desktop$\output.csv")
bee 2000 200
