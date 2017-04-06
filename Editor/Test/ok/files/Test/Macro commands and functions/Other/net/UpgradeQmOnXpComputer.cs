 The following four lines is what you should change
str setup="\\gintaras\e\myprojects\app\quickmac.exe" ;;QM setup program as it is seen from a client computer.
str computers="xp" ;;list of computers
str password="p" ;;assume, all computers use same password (Options -> Security -> Network)
str cmdline="/silent /nocancel" ;;install without the setup wizard, using the same settings as when installing last time
 _______________________

str sm=
 function $setup $cmdline
 run setup cmdline

str c; int r n
foreach c computers
	r=net(c password "NewItem" 0 "NetUpgradeQm" sm "Function" "" "\User\Temp" 17) ;;send macro that runs the setup program
	if(!r) r=net(c password "NetUpgradeQm" 0 setup cmdline) ;;if ok, run the macro
	if(r) out "Cannot upgrade on %s (error %i)" c r
	else out "Upgraded on %s." c; n+1

if(n) out "This may require to restart Windows on some computers (you will see a message box)."
