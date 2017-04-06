 /VR_Main
function

str voicecommands
if(!rget(voicecommands)) voicecommands="Voice Commands"; rset voicecommands
 getmacro
str s.getmacro(voicecommands)
err inp- voicecommands "Where is list of voice commands? Please type QM item name."; goto getmacro

HSRLib.Vcommand- vr
int- vrmenu
vr._create
vr._setevents("vr_Events")
vr.Microphone=0
vr.Initialized=1
if(vrmenu)
	vr.Deactivate(vrmenu)
	vr.ReleaseMenu(vrmenu)
vrmenu=vr.MenuCreate("qm" "main" 4)

int i j; str sl 
for(i 0 2000000000)
	if(sl.getl(s -i)<0) break
	if(!sl.len or !isalpha(sl[0])) continue
	vr.AddCommand(vrmenu i+1 sl sl voicecommands 0 sl)

vr.Activate(vrmenu)
