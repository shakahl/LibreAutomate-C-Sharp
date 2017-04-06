lpstr vbs=
 Function Func(ByVal a)
 MsgBox a, , "VbScript Message"
 Func="From Func"
 End Function
VbsAddCode vbs

str s1="From QM"
str s2=VbsFunc("Func" s1)
mes s2
