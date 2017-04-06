out
out RunConsoleCallback(&sub.OnConsoleOutput 0 "$my qm$\console2.exe /ab cd")
 out RunConsole2("$my qm$\console2.exe /ab cd")


#sub OnConsoleOutput
function# cbParam $s
out F"<{s}>"
