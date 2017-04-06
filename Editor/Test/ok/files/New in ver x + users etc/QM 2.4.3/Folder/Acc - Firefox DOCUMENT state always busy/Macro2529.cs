Acc a
int w=win("" "MozillaWindowClass")
PF
a.Find(w "PAGETAB" "Omnitel - Padengimo žemėlapis" "" 0x1001 1)
PN;PO
a.DoDefaultAction
PF
rep 40
	a.FromWindow(w OBJID_CLIENT); a.a=a.a.Navigate(0x1009 1)
	0.01
 a.Find(w "DOCUMENT" "Omnitel - Padengimo žemėlapis" "" 0x1091 3)
PN
a.Find(a.a "TEXT" "Įrašykite miesto, adreso pavadinimą" "state=0x101000 0x20000040" 0x1015 9)
 a.Find(w "TEXT" "Įrašykite miesto, adreso pavadinimą" "state=0x101000 0x20000040" 0x1015 9)
PN;PO
