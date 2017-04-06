 Currently this is called from macro "VS on save"

int w=win("CatkeysHelp" "HwndWrapper[DefaultDomain;*" "DEVENV" 1)
if(w=0) ret

spe 10
int wa=win
Acc a.Find(w "SCROLLBAR" "" "" 0x10A0)
act w
str scrollPos=a.Value
key F5
if(val(scrollPos)) a.SetValue(scrollPos); err
act wa

 notes:
  this does not work
 Acc a1.Find(w "PUSHBUTTON" "Refresh" "" 0x1001)
 a.DoDefaultAction ;;error "member not found", even when window active
 
  this works only when window active
 SendKeysToWindow w key(F5)
