str s2="joe name daniel name megan name joan"
ARRAY(str) arr2; int i

str r=s2; r.findreplace(" name " "[]")
arr2=r
for(i 0 arr2.len) out arr2[i]
