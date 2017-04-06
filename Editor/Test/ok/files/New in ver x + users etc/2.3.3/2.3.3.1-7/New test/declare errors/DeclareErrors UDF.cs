
ShowDialog ""
SendMail "" "" ""
Function188 0 0
Acc a.DoDefaultAction


 error not in caller
call &func_cb
EnumWindows &func_cb 0

ARRAY(str) ar
foreach ar "A:C" FE_ExcelRow
	out "%s %s %s" ar[0] ar[1] ar[2]
