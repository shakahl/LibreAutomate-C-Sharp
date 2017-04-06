 \Dialog_Editor

 sub.ThreeSub

 ref SCI


#sub ThreeSub c
function [x] [y] ;;hjhjh
 Annotation of ThreeSub.

int u1
out 33
if(0) end "aaa"; act "bbb"
#out "compiling ThreeSub"
 #warning "test warn"
 str s1 s2; s1=s1+s2 ;;warning
 out Statement(2 0 _s); out _s
sub.Sub4
 k
 out EDGE_BACKGROUND

 out 3
 sub.ThreeSub


#sub Sub4
function# [x] ;;pdpdp
 ExternSub k.Priv; ret
 m
out 44
#out "compiling Sub4"
GetCallStack


#sub Thread
mes 2
#out "compiling Thread"


 #sub ap_SheetSelectionChange ;;not supported
 function IDispatch'Sh Excel.Range'Target ;;Excel._Application'ap
 out 1


#sub Move
 function k
out "move2"
#out "compiling Move"


#sub Dialog

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Static 0x5400000E 0x0 8 8 16 16 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "3"
str sb3
sb3="resource:<ExternSub.__subs>Copy.gif"
if(!ShowDialog(dd 0 &controls)) ret
