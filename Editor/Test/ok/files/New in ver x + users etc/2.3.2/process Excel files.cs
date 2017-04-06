 Before you run this macro:
 Excel should not be running.
 Make sure that documents will be maximized, so that document text is displayed in title bar.
 I have Office 2003. If your Office version is other, this macro may not work.

 ______________________________________________

 repeat for each excel file
Dir d
foreach(d "$Documents$\*.xls" FE_Dir)
	str sPath=d.FileName(1)
	
	 open, skip or abort?
	str message.from("Open and manipulate this file?[][]" d.FileName)
	sel(mes(message "" "YNC"))
		case 'C' ret
		case 'N' continue
	
	 open
	str cl.from("''" sPath "''")
	run "excel" cl
	
	 wait for document
	str fn.getfilename(d.FileName) ;;remove extension
	str excelTitleBarText.from("Microsoft Excel - " fn)
	int w1=wait(30 WA win(excelTitleBarText "" "" 2))
	
	mes "Ready?"
	
	'CH ;;select first cell
	'A{}
	'icRRDDDDRSD A{}
	'oe
	5 win("Format Cells" "bosa_sdm_XL9")
	'R A{mm}
	'Y
	act w1
	'LR Cc
	'DDDDLLL Cv
	'RRRUUU Cc
	'DDDLL Cv
	'LSR Cc
	'S{REDL} Cv
