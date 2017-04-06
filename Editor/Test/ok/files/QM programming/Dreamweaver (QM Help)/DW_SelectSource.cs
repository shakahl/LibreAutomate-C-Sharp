 /
function [str&sGetSel]

int i n(100)
for i 0 n
	_s.getwinclass(child); err
	if(_s~"code view") break
	if(i=0) key C`
	0.05
if(i=n) mes- "Failed to focus code view"
0.1

 select whole lines
Acc a.Find("" "PUSHBUTTON" "Outdent Code" "class=Button[]id=27021" 0x1005)
a.DoDefaultAction
0.1
if(&sGetSel) sGetSel.getsel
