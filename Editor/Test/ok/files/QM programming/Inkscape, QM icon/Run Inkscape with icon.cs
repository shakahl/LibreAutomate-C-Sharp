run "$program files$\Inkscape\inkscape.exe" F"''{_s.expandpath(`$qm$\icons\icon.svg`)}''"
int w=wait(0 WA win("icon.svg - Inkscape" "gdkWindowToplevel"))
rep
	1; max w; err continue ;;sometimes error 'hung window'
	break
key CSf Avn ;;Colors etc, Icons preview
1
wait 0 S "image:h8E5376D0" w 0 1|16|0x400
mou- 100 0
lef
mou


 lef 0.933 0.300 w 5 ;; 'icon.svg - Inkscape', "~:667BF622"
