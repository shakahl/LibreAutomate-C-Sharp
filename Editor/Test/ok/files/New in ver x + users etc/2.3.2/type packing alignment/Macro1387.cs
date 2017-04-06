out
str h="$temp$\test.h"
str s=
 struct  T0  { BYTE a;  int  b; WORD c; };
 # pragma   pack ( 1 )  //comm
 struct  T1  { BYTE a;  int  b; WORD c; };
 struct  T3  { T1 a; double b; };
 struct  T4  { BYTE a; struct { BYTE b; BYTE c; }; WORD c; };
   #pragma pack()
 #pragma pack(push)
 #pragma pack(push, 2)
 #pragma pack(push, aaa, 4)
 #pragma pack(push, bbb)
 struct  T2  { BYTE a;  int  b; WORD c; };
 #pragma pack(pop, bbb)
 #pragma pack(pop, aaa, 1)
 #pragma pack(pop)
 #pragma pack(pop, 2)
;
s.setfile(h)

str incl="C:\Program Files\Microsoft SDKs\Windows\v7.0\Include"
str t="$temp$\test.txt"

ConvertCtoQM h t incl "" 4|64|128 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"
out s.getfile(t)

 ConvertCtoQM h t incl "" 2 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"
 out s.getfile("$temp$\test_preprocessed.txt")
