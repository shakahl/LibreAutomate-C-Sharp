 run "$program files$\Lynx - web browser\lynx.exe" "http://www.quickmacros.com" "" "*" 0x10000

out
str txt
str lynxDir="$program files$\Lynx - web browser"
str url="http://www.quickmacros.com"
RunConsole2 F"{lynxDir}\lynx.exe -dump -nolist {url}" txt lynxDir
out txt

 -nolist removes [link number] and link URLs
