opt err 1
run "$program files$\Process Explorer\procexp.exe" "" "" "" 0x10800 "Sysinternals"
run "notepad.exe"
 spe 500
opt waitcpu 1
run "iexplore.exe" "http://www.quickmacros.com/wiki/tiki-editpage.php"
run "winword.exe"; 1
run "mailto:gindi@takas.lt"
run "dreamweaver.exe" "$desktop$\test.htm"
run "firefox.exe" "$qm$\textarea.htm"
run "$program files$\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.exe"
