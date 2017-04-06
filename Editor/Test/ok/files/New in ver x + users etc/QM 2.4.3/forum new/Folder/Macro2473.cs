 int ec=CreateProcessSimple("''$my qm$\this_is_my_exe.exe'' /a ''b''" 1)
 out ec

 str progPath.expandpath("$my qm$\this_is_my_console_exe.exe")
 str s
 RunConsole2 F"''{progPath}'' /a ''b''" s
 out s

str s
RunConsole2 "''$my qm$\this_is_my_console_exe.exe'' /a ''b''" s
out s
