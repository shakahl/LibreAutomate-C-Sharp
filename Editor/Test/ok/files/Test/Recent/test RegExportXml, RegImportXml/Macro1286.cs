out
str f1 f2 k1 k2 s
 k1="Software\GinDi\QM2\settings"
 k2="Software\GinDi\QM2\settings2"
k1="Software\GinDi\QM2\user"
k2="Software\GinDi\QM2\user2"
 k1="Software\GinDi\QM2"
 k2="Software\GinDi\QM2"

RegExportXml "$desktop$\r.xml" k1

out _s.getfile("$desktop$\r.xml")
 ret

RegImportXml "$desktop$\r.xml" k2


 compare
f1.expandpath("$desktop$\r1.reg")
f2.expandpath("$desktop$\r2.reg")
del- f1; err
del- f2; err
run "regedit.exe" s.format("/E ''%s'' ''HKEY_CURRENT_USER\%s\''" f1 k1) "" "" 0x400|0x10000
run "regedit.exe" s.format("/E ''%s'' ''HKEY_CURRENT_USER\%s\''" f2 k2) "" "" 0x400|0x10000

run "$program files$\ExamDiff\ExamDiff.exe" s.format("''%s'' ''%s''" f1 f2)
