 /
function ~cmd

int h = FindWindow("PowerProMain" 0)
if(!h)
	run "$Program Files$\PowerPro\powerpro.exe"
	for(_i 0 30) 0.1; h = FindWindow("PowerProMain" 0); if(h) break
	if(!h) end "can't connect to ppro"

type COPYDATASTRUCT dwData cbData lpData
COPYDATASTRUCT cd
cd.cbData=cmd.len+1
cd.dwData=1
cd.lpData=cmd

SendMessage(h WM_COPYDATA 0 &cd)
