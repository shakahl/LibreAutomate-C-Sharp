spe
key W
int h
if(_winver<0x501 or (rget(_s "ShellState" "Software\Microsoft\Windows\CurrentVersion\Explorer" 0 "" REG_BINARY) and _s[32]&2=0)) ;;classic
	h=wait(10 WV win("" "BaseBar" "" 128 0x94800000 0))
else ;;XP/Vista
	h=wait(10 WV win("" "DV2ControlHost"))
mov xm ym h
