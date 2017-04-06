 /
function hDlg

 Removes theme from all push-like checkboxes.

ARRAY(int) a; int i j
child "" "Button" hDlg 16 0 0 a
for(i 0 a.len)
	j=GetWinStyle(a[i])
	if(j&BS_PUSHLIKE) sel(j&BS_TYPEMASK) case [BS_AUTOCHECKBOX,BS_AUTORADIOBUTTON,BS_CHECKBOX,BS_RADIOBUTTON] TO_NoTheme a[i]
