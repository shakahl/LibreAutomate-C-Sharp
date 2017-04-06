 /
function ARRAY(str)&a $_file

 Saves 2-dim str array to CSV file.


opt noerrorshere 1
ICsv x._create
x.FromArray(a)
x.ToFile(_file)
