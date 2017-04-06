 run "Q:\My QM\Macro1928.exe" "ff"
 CreateProcessSimple "Q:\My QM\Macro1928.exe ff"
 CreateProcessSimple2 "Q:\My QM\Macro1928.exe"
 CreateProcessSimple2 "Q:\My QM\Macro1928.exe" "ff"

 SetCurDir "Q:\My QM"
 CreateProcessSimple "Macro1928.exe ff"

 #ret
str s
 s="Q:\My^ QM\Macro1928.exe"
 s="Q:\My^ QM\Macro1928.exe /cl"
s="cd Q:\My^ QM[]Macro1928.exe /cl"
str bat.expandpath("$temp$\test.bat")
s.setfile(bat)
run bat
