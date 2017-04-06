int w=win("" "MozillaWindowClass")
Acc a.Find(w "PAGETAB" "Omnitel - Padengimo žemėlapis" "" 0x1001)
a.DoDefaultAction
PF
 a.Find(w "DOCUMENT" "Omnitel - Padengimo žemėlapis" "" 0x13001 2)
a.Find(w "TEXT" "Įrašykite miesto, adreso pavadinimą" "state=0x101000 0x20000040" 0x13015 5)
PN;PO
