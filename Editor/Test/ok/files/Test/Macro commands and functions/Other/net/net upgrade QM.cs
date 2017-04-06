 This macro upgrades QM on several computers.

str setupfile="$qm$\quickmac.exe" ;;QM setup file. Macro will send and run it on each computer.
 str computers="computer1[]xp[]computer3" ;;list of computers
 str computers=ip
str computers="xp"
str password="p" ;;assume, all computers use same password
str cmdline="/silent" ;;install without the setup wizard, using the same settings as when installing last time

str c sd.getfile(setupfile)
int r

str sm=
 function str'filedata $cmdline
 str temp="$temp$\quickmac.exe"
 filedata.setfile(temp)
 run temp cmdline

foreach c computers
	r=net(c password "NewItem" 0 "NetUpgradeQm" sm "Function" "" "\User\Temp" 17) ;;send macro that receives and runs setup file
	if(!r) r=net(c password "NetUpgradeQm" 0 sd cmdline) ;;if ok, run the macro
	if(r) out "Cannot upgrade QM on %s (error %i)" c r
	else out "QM upgraded on %s" c

