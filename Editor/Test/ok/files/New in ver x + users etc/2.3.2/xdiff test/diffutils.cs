out
str f1.expandpath("$desktop$\threads.cpp")
str f2.expandpath("$qm$\threads.cpp")
 str fPatch.expandpath("$desktop$\xdiff_patch.txt")
str diff.expandpath("$program files$\GnuWin32\bin\diff.exe")

str cl.format("''%s'' -u ''%s'' ''%s''" diff f1 f2)
 out cl
RunConsole2 cl
