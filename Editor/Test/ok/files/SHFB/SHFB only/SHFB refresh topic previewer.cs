spe 10
int w=win("*Sandcastle Help File Builder" "*.Window.*" 0 1)
act w

 get vertical scrollbar position
int c=child("" "*.Window.*" w 0x400 "wfName=ehTopicPreviewerHost")
Acc a.Find(c "SCROLLBAR" "" "" 0x10A4)
str scrollPos=a.Value

key F5

 restore vertical scrollbar position
if(val(scrollPos)!=0) a.SetValue(scrollPos); err

SHFB_focus_editor
