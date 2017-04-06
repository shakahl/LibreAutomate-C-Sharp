 run "$program files$\Lynx - web browser\lynx.exe" "http://www.quickmacros.com" "" "*" 0x10000

out
str txt
str lynxDir="$program files$\Lynx - web browser"
str url="http://www.quickmacros.com"
SetCurDir lynxDir
RunConsole F"{lynxDir}\lynx.exe" "" txt
out txt

 ERRORS:
 
 Redirection is not supported.
