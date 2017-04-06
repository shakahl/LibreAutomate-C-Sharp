str s
int w=firefox_address_from_mouse_and_select_map_tab(s "maps\.lt")
Acc a.FindFF(w "INPUT" "" "id=dijit_form_TextBox_0" 0x1004 30)
a.Select(3)
 key Ca (s) Y
key Ca; paste s; key Y

err+ OnScreenDisplay _error.description
