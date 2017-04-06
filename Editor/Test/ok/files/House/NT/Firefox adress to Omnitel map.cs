str s
int w=firefox_address_from_mouse_and_select_map_tab(s "Mobilaus interneto padengimo žemėlapis - Omnitel")
 Acc a.Find(w "TEXT" "Įrašykite miesto, adreso pavadinimą" "state=0x101000 0x20000040" 0x13015 20) ;;old
Acc a.FindFF(w "INPUT" "" "id=dijit_form_TextBox_0[]placeholder=Įrašykite adresą" 0x11084 30)
a.Select(3)
 key Ca (s) Y ;;flickers and makes small map
key Ca; paste s
 0.2 ;;sometimes does not update map. But this does not help.
key Y

err+ OnScreenDisplay _error.description
