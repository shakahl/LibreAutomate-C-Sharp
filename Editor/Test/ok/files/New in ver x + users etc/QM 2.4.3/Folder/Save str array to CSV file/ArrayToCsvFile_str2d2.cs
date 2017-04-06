 /
function ARRAY(str)&a $_file [ARRAY(str)&aAppendRow]

 Saves 2-dim str array to CSV file.

 aAppendRow - optional 1-dim array containing data for a new row to append to the end of the CSV file.
   Array element count must be >=1 and <= CSV column count.


opt noerrorshere 1
ICsv x._create
x.FromArray(a)
if(&aAppendRow) x.AddRowSA(-1 aAppendRow.len &aAppendRow[0])
x.ToFile(_file)
