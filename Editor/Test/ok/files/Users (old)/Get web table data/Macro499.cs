ClearOutput

 Get first cell.
 Acc a=acc("Order #" "TEXT" win(" Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1801 0x40 0x20000040 "parent next6 first")
 Acc a=acc("WavePad 3.05" "LINK" win(" Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1001)
 Acc a=acc("512MBDDRII66764Mx8" "TEXT" win(" Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1801 0x40 0x20000040)
 Acc a=acc("Gamintojas" "LINK" win(" Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1001)
 Acc a=acc("Komponentai" "LINK" win(" Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1001)
Acc a=acc("Order #" "TEXT" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1801 0x40 0x20000040 "parent next6 first")

ARRAY(str) an av
a.WebTableToArray(an av 1 "(*)")
 Results.
int i
for i 0 an.len
	out "%-50s VALUE=%s" an[i] av[i]
 for i 0 an.len
	 out an[i]
 for i 0 av.len
	 out av[i]
	