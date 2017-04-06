str sf1="G:\speed.txt"
str sf2="E:\speed.txt"

str s.all(10*1024*1024 2)
PF
s.setfile(sf1)
PN
s.setfile(sf2)
PN;PO

DeleteFileCache sf1
DeleteFileCache sf2
0.5

PF
s.getfile(sf1)
PN
s.getfile(sf2)
PN;PO
