
// Updates the buttons on the status bar periodically

str trueico = "$qm$\favorites.ico"
str falseico = "$qm$\close.ico"

int+ monitor = 1
int i
rep

	WMB_UpdateButtonIcon(1 "WMB_CheckWinActive" trueico falseico);
	WMB_UpdateButtonText(2 "WMB_GetWinRect")
	wait 0.5
	
	if(!monitor)
		break