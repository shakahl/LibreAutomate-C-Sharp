typelib Excel {00020813-0000-0000-C000-000000000046} 1.2
Excel.Application ex1._getactive

int Exl=win("Microsoft Excel - final fp list (use this one to set up FP).xls")
int IntExp=act(win("PEDS - Microsoft Internet Explorer provided by CS&O"))

Excel.Worksheet ws=ex1.Worksheets.Item("Init")
Excel.Range r1=ex1.Range("a1")

Acc RefreshButton=acc("Refresh" "PUSHBUTTON" IntExp "Internet Explorer_Server" "" 0x1001)
Acc AddButton=acc("Add" "PUSHBUTTON" IntExp "Internet Explorer_Server" "" 0x1001)

str initt
str ccount

act Exl
ws.Select
r1.Select
ccount.getsel
rep
	spe
	if(val(ccount) = 0) break
	'R
	initt.getsel
	'LD
	ccount.getsel
	spe 500
	act IntExp
	RefreshButton.DoDefaultAction
	 wait 20 I
	 deb
	 'S{TTTT}
	 deb-
	 initt.setsel
	AddButton.DoDefaultAction
	act Exl
	ccount.getsel