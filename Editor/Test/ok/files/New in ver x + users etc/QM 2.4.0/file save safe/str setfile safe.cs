str sf="$my qm$\test\safe.txt"
str s.all(30000 2 'a')

PF
 s.setfile(sf)
 s.setfile(sf 0 -1 0x100)
s.setfile(sf 0 -1 0x200)
PN;PO
