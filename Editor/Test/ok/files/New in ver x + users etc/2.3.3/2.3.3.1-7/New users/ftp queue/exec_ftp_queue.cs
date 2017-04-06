 Opens file created by dlg_ftp_queue, and uploads all.

str sFile="$my qm$\ftp queue.csv"

ICsv x=CreateCsv(1)
ICsv xe=CreateCsv(1) ;;for failed uploads

x.FromFile(sFile)

int i
for i 0 x.RowCount
	 get file and ftp from csv
	str s1 s2
	s1=x.Cell(i 0)
	s2=x.Cell(i 1)
	out "file: %s,    ftp: %s" s1 s2
	
	 now upload
	int failed=0
	err-
	 ...
	 ...
	err+ failed=1
	if(failed) xe.AddRowSA(-1 2 &s1)

if xe.RowCount
	xe.ToFile(sFile) ;;save failed uploads. Then you can run this macro again to upload them.
else
	del- sFile
