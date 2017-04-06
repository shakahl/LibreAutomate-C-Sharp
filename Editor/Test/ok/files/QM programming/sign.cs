function# $files

 Signs all QM exe and dll, except setup.
 Inno Setup now is configured to run this.
   It runs this twice.
   Before adding files it signs uninstaller. Then this func also signs all our files. Also calls before_InnoSetup.
   After creating setup it signs setup file.
 Also this func has shell menu trigger...
 note: disable antivirus realtime protection. It sometimes locks some files and signtool gives error. Excluding does not work.

int Case=0
str f clPlus

if !empty(files) ;;from shell menu
	f=files
	if(!f.beg("''")) f-"''"; f+"''"
	goto g1

f=
 sign\qm.exe
 sign\qmserv.exe
 sign\qmrun.exe
 sign\qmmacro.exe
 sign\qmdd.exe
 sign\qmcl.exe
 sign\qmtul.exe
 sign\qmhook32.dll
 sign\qmshex32.dll
 sign\qmshex64.dll
 sign\qmtc32.dll
 sign\qmtc64.dll
 sign\Portable\QuickMacrosPortable.exe

sel _command 3
	case "!''*.tmp''" ;;Inno setup signs uninstaller before adding files. Let's sign our files + uninstaller.
	_s=_command; f.addline(_s.trim("!''")); Case=0
	sub.before_InnoSetup
	case "!''*''" f=_command+1; goto g1 ;;Inno setup signs setup file
	case else if(_command) out "unsupported command line"; ret 1

sel Case
	case 0
	 some files are locked, therefore we copy all to the sign folder and sign the copies. Then Inno Setup script adds the copies to the setup file.
	foreach _s f
		if(_s.beg("sign\")) cop- F"$qm$\{_s+5}" F"$qm$\{_s}"
	f.findreplace("[]" " ")
	
	case 1 ;;drivers
	clPlus="/ac ''sign\DigiCert High Assurance EV Root CA.crt'' /v" ;;use cross-cert; verbose
	 f="sign\qmphook.sys" ;;test
	f="qmphook.sys x64\qmphook.sys"
	 info: to sign drivers, I just downloaded the cross-cert from digicert and use it in command line. And it works. Ordered the main cert from digicert through ksoftware. It seems that it can be used to sign user and kernel code.
	
	 case 2 ;;setup. Don't do it here. Inno Setup script is configured to call this exe; it signs installer and uninstaller.
	 f="quickmac.exe"

 g1
ChDir "q:\app"
str signtool.expandpath("c:\Program Files\Microsoft SDKs\Windows\v7.0\Bin\signtool.exe")
str timeserver="http://timestamp.digicert.com"
 RunConsole2 F"''{signtool}'' sign /?"
int R=RunConsole2(F"''{signtool}'' sign /f sign\digicert.p12 /p jucag&45 /tr {timeserver} /td sha256 /fd sha256 {clPlus} {f}" _s "" 0x100)
_s.findreplace("[][]" "[]"); _s.trim
if(_s.beg("Done ")) _s.getl(_s 1 2)
OnScreenDisplay _s _s.len/150+5 0 -100 0 10 1 5 "sign" 0xa0ffff
ret R

 RunConsole2 F"''{signtool}'' sign /f sign\certum.p12 /p slapta1 /t http://time.certum.pl {f}" ;;using free open-sorce-developer cert from certum


#sub before_InnoSetup
 Called by 'sign' wben Inno Setup calls it at the beginning of creating QM setup file.

 Write System folder MD5 to setup.ini etc.
SendMessage _hwndqm WM_USER+25 0 0

 Copy System.qml to 'sign' subfolder and make read-only, non-WAL.
str sysfrom="$qm$\System.qml"
str syscopy="$qm$\sign\System.qml"
FileCopy sysfrom syscopy
FileCopy "$qm$\Main.qml" "$qm$\sign\Main.qml"

Sqlite x.Open(syscopy 0 4)
int uv=x.ExecGetInt("PRAGMA user_version")
x.Exec(F"PRAGMA journal_mode=DELETE;PRAGMA user_version={uv|0x100000}")
x.Close
