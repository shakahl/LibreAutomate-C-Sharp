out

SetThreadMenuIcons "1=2 2=11 3=12 7=3 8=4" "$qm$\il_qm.bmp" 1

str s=
 ...............
 |
 1 &Normal
 2 &Checked
 3 &Radio
 -
 4 &No icon
 5 &No icon, checked
 6 &No icon, radio
 -
 >7 Submenu
 	8 Normal2
 	<

MenuPopup m.AddItems(s)
m.CheckItems("2 5")
m.CheckRadioItem(3 3 3)
m.CheckRadioItem(6 6 6)
int i=m.Show(0 0 1)
out i
