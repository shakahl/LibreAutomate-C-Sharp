 /
function ARRAY(str)&a $_file

 Loads 1-dim str array from CSV file.


opt noerrorshere 1
ICsv x._create
x.FromFile(_file)
a.create(x.RowCount)
for(_i 0 a.len) a[_i]=x.Cell(_i 0)
