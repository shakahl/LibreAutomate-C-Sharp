 out
str s1 s2; int i1 i2
PF
rget i1 "unicode" "software\gindi\qm2\settings"
PN
rget i2 "acclog flags" "software\gindi\qm2\settings"
PN
rget s1 "file" "software\gindi\qm2\settings"
PN
rget s2 "backups" "software\gindi\qm2\settings"
PN;PO
out i1
out i2
out s1
out s2

