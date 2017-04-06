 /
function ARRAY(str)&a $_file

 Saves 1-dim str array to CSV file.


opt noerrorshere 1
ICsv x._create
for(_i 0 a.len) x.AddRowSA(-1 1 &a[_i])
x.ToFile(_file)
