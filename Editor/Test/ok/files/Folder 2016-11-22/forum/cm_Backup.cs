OnScreenDisplay "Backing up, please wait..." -1 0 -1 "" 0 0x00000f 8|1 "cm_Backup" 0x00ffff
mes "in cm_Backup"

err+
	OsdHide "cm_Backup"
	end _error
OsdHide "cm_Backup"
