str mono.expandpath("$program files$\Mono\lib\mono\4.5\mcs.exe")
str csFile.expandpath("Q:\Test\test.cs")
PF
 RunConsole1 F"''{mono}'' -noconfig ''{csFile}''"
RunConsole1 F"''{mono}'' -?"
PN
PO
