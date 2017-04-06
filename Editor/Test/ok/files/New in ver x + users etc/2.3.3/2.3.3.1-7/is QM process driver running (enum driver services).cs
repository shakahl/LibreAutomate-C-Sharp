out
Services.clsServices ses._create
int st=Services.Kernel_and_File_System_Drivers_20202020
ses.ServicesEnumType=&st
Services.clsService se
int n
foreach se ses
	n+1
	 out se.DisplayName
	str s=se.DisplayName
	if(s~"QM process triggers") out "QM driver running"; ret
if(n) out "QM driver not running"
else out "failed"
