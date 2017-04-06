 /
function ARRAY(str)&a $_file

 Loads 2-dim str array from CSV file.


opt noerrorshere 1
ICsv x._create
x.FromFile(_file)
x.ToArray(a)
