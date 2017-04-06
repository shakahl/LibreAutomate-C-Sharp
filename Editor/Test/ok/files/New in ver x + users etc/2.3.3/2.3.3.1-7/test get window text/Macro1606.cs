dll "qm.exe" TestGT h noInternal str*s1 str*s2 str*s3

 ----
 int w=win("Dialog" "#32770")
 int c=id(3 w)
 ----
 int w=win("" "QM_Editor")
 int c=id(2216 w)
 ----
 int w=win("Options" "#32770")
 int c=id(1103 w)
 ----
int w=win("Dialog" "#32770")
int c=id(4 w)

str s1 s2 s3
TestGT c 0 &s1 &s2 &s3
out s1
out s2
out s3
