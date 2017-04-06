out
str sf="Q:\my qm\test\backup.qml"
str sf2="Q:\my qm\test\ok.qml"
str sf3="Q:\my qm\test\ok2.qml"
str sfPatch="Q:\my qm\test\ok.diff"

out Crc32(sf 0 1)
out Crc32(sf2 0 1)

#compile "__Xdiff"
Xdiff x

x.DiffBinaryFile(sf sf2 sfPatch)

out "---------------------"

x.PatchBinaryFile(sf sfPatch sf3)

out Crc32(sf3 0 1)
