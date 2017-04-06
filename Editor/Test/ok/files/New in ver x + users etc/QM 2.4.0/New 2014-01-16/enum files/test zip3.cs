out
str z.expandpath("$temp$\test.zip")
iff(z) del- z
zip z "$My QM$\test"
 zip z "$My QM$\test\*.qml" 4
 zip z "$My QM$\test\export.qml" 4
 zip z "$My QM$\test" 0 0 "qml"
 zip z "$My QM$\test-no" 0 _s; out _s; ret
 run z
run "$program files$\7-Zip\7zFM.exe" z "" "*" ;;7-Zip File Manager
