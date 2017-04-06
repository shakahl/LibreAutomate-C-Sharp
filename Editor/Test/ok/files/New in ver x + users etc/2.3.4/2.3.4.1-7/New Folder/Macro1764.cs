int w=win("Options" "#32770") ;;QM Options dialog
Acc a=acc("" "PROPERTYPAGE" w "#32770" "" 0x1004)
a.Navigate("first")
rep
	out a.Name
	a.Navigate("next"); err break
