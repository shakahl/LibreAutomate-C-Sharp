 close all toolbars, and remember their names and owner windows

ARRAY(int) at ;;toolbar handles
ARRAY(int) aw ;;toolbar owner window handles
ARRAY(str) an ;;toolbar names
win "" "QM_toolbar" "qm" 0 "" at
aw.create(at.len); an.create(at.len)
int i
for i 0 at.len
	aw[i]=GetToolbarOwner(at[i])
	an[i].getwintext(at[i])
	clo at[i]


 create all toolbars
for i 0 an.len
	if(aw[i]) mac an[i] aw[i]; else mac an[i]
