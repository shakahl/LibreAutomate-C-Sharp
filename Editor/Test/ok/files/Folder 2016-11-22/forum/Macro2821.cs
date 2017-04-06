str s
HtmlDoc k.InitFromText(s)
ARRAY(str) a
k.GetTable(0 a)
int i nc=7
ICsv x._create; x.ColumnCount=nc
for i 0 a.len/nc
	str& firstCell=a[i*nc]
	firstCell.rtrim("")
	x.AddRowSA(i nc &firstCell)

str csv; x.ToString(csv); out csv
x.InsertColumn(column_index_or_minus_one)
