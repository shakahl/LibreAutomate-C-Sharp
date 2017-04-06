 str sf="$temp$\test_attr.txt"
 _s="test"; _s.setfile(sf)
 FileSetAttributes sf FILE_ATTRIBUTE_READONLY|FILE_ATTRIBUTE_HIDDEN 1
 out "0x%X" FileGetAttributes(sf)
 run sf "" "properties" "" 0x70000
 1; del- sf

  set last-access time to time-now
 str sf="$my qm$\test\test.txt"
 DateTime t.FromComputerTime(1)
 FileSetAttributes sf 0 3 0 0 t
 run sf "" "properties" "" 0x70000

str sf="$temp$\test_attr.txt"
_s="test"; _s.setfile(sf)
DateTime t.FromComputerTime(1); t.AddYears(1)
FileSetAttributes sf FILE_ATTRIBUTE_READONLY|FILE_ATTRIBUTE_HIDDEN 1 t
out "0x%X" FileGetAttributes(sf)
run sf "" "properties" "" 0x70000
1; del- sf
