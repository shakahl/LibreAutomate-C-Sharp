 /DEX_Main3
function hDlg hlv reinit

DEX_DATA& d=+DT_GetParam(hDlg)

if(!reinit)
	SetWinStyle hlv LVS_REPORT|LVS_SHAREIMAGELISTS|LVS_SINGLESEL 1
	SendMessage hlv LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_INFOTIP|LVS_EX_FULLROWSELECT|LVS_EX_ONECLICKACTIVATE -1
	
	SendMessage hlv LVM_SETIMAGELIST LVSIL_SMALL d.il
	
	DEX_LvAddCol2 hlv 0 "Item" 70
	DEX_LvAddCol2 hlv 1 "Subitem 1" 70
	DEX_LvAddCol2 hlv 2 "Subitem 2" 70
else
	SendMessage hlv LVM_DELETEALLITEMS 0 0

DEX_LvAdd2 hlv 0 3001 7 "One" 1 2
DEX_LvAdd2 hlv 1 3002 8 "Two" 3 4
DEX_LvAdd2 hlv 2 3003 9 "Three" 5 6
