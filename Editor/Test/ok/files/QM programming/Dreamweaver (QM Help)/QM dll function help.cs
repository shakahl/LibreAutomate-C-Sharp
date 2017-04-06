ifa- "Dreamweaver"
	mes- "Dreamweaver must be active"
OnScreenDisplay "click QM dll function name in Dreamweaver text" 0 0 0 0 0 0 1
wait -1 ML
OsdHide
lef ;;will be double click; triple click would select whole line
str s.getsel
act _hwndqm
mac+ "QmDll"
men 2004 _hwndqm ;;Find...
int hf=win("Find" "#32770" _hwndqm 32)
if(hf) act hf
else hf=child("Find" "#32770" _hwndqm)
EditReplaceSel hf 1127 s 1
but+ 1026 hf; but+ 1027 hf; but- 1028 hf
0.5
but id(1129 hf)
0.1
key HLSE
FuncHelp
act "Dreamweaver"
DW_SelectSource
key Cv
key Y
key C`
