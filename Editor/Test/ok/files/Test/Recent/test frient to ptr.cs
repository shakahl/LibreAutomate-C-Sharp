BSTR b="abc"
 word* p=+b
word* p=b
out b.pstr
out p

 ARRAY(str) a.create(1)
  SAFEARRAY* sap=+a
 SAFEARRAY* sap=a
 out a.psa
 out sap

 str s="s"
 SAFEARRAY* sap2=s
 out "%i" s
 out sap2

 int* ip=+3
 SAFEARRAY* sap3=ip
 out ip
 out sap3

 FLOAT f=0.5
 double d=f
 out d

 CURRENCY c=6
 long l=c
 out l
 out c

 DATE da=5
 double d=da
 out d

 DECIMAL de=8
 int i=de
 out i

 byte* b=_s

 type BSTR2 word*p
 BSTR2 b
 word* p=b
