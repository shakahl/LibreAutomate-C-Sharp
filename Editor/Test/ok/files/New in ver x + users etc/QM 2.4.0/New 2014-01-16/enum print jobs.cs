str printerName="HP Deskjet F300 Series"

int hp
if(!OpenPrinter(printerName &hp 0)) end _s.dllerror
ARRAY(JOB_INFO_1) a.create(100); int i n
if(!EnumJobs(hp 0 a.len 1 &a[0] sizeof(JOB_INFO_1)*a.len &_i &n)) _s.dllerror; ClosePrinter hp; end _s
out n
out sizeof(JOB_INFO_1)*a.len
out _i
for i 0 n
	out a[i].pDocument
	 out _s.getstruct(a[i] 1)
ClosePrinter hp
