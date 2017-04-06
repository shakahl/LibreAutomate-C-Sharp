\Dialog_Editor

 Portable QM Setup.
 _________________________________________

function# hDlg message wParam lParam
if(hDlg) goto messages

int silent=lParam ;;if lParam nonzero, does not show message boxes and does not overwrite main.qml
str mtit="Portable QM Setup"

 1. DIALOG.

type QP_DIALOG ~controls ~e4dri ~e6fil
QP_DIALOG d.controls="4 6"

 get saved settings
rget _s "qmpe3"; _s.setstruct(d); err
if(!d.e4dri.len) ;;find first removable drive
	ARRAY(Wsh.Drive) a; int i
	if(GetDrives(a 1)) for(i 0 a.len) sel(_s.from(a[i].Path) 1) case ["A:","B:"] case else break
	if(i<a.len)
		d.e4dri=a[i].Path
		if(dir(_s.from(d.e4dri "\PortableApps") 1)) d.e4dri=_s

if(!ShowDialog("" &QP_Setup &d)) ret
rset _s.getstruct(d) "qmpe3"

d.e4dri.rtrim('\'); if(d.e4dri.len=1) d.e4dri+":"
if(!d.e4dri.len or !dir(d.e4dri 1)) mes- "Destination does not exist." mtit "x"
d.e6fil.replacerx("([]){2,}" "[]")


 2. COPY FILES, ETC.

OnScreenDisplay "Please wait..." -1 0 0 0 0 0 9

 Folders and files in destination:
 \QuickMacrosPortable
 \QuickMacrosPortable\qm_pe.exe
 \QuickMacrosPortable\qmpe_reg.dat
 \QuickMacrosPortable\App
 \QuickMacrosPortable\App\*everything from current QM folder*
 \QuickMacrosPortable\App\AppInfo
 \QuickMacrosPortable\App\AppInfo\appinfo.ini
 \QuickMacrosPortable\App\AppInfo\appicon.ico
 \QuickMacrosPortable\Data
 \QuickMacrosPortable\Data\My QM
 \QuickMacrosPortable\Data\My QM\*optional files*
 \QuickMacrosPortable\Data\My QM\Main.qml

str s1 s2 sfroot sfapp sfappinfo sfdata sfmyqm sfsrc curfile
sfroot.from(d.e4dri "\QuickMacrosPortable")
sfapp.from(sfroot "\App")
sfappinfo.from(sfapp "\AppInfo")
sfdata.from(sfroot "\Data")
sfmyqm.from(sfdata "\My QM")
sfsrc.expandpath("$qm$")
if(sfsrc~"q:\app") sfsrc="c:\program files\quick macros 2"

mkdir sfappinfo
mkdir sfmyqm

 make qm_pe.exe and move to usb drive
wait -1 H mac("QP_Exe" "" 1) ;;/exe makes exe, which then runs and exits
ren- "$my qm$\qm_pe.exe" sfroot

 export qm registry settings
if(QP_ExportReg(sfroot curfile)) err mes- "Failed to export QM registry settings." mtit "x"

 copy qm folder
cop- _s.from(sfsrc "\*") sfapp 0x2C0~FOF_FILESONLY

 create appinfo.ini
s1=
 [Format]
 Type=PortableApps.comFormat
 Version=1.0
 
 [Details]
 Name=Quick Macros Portable
 Publisher=www.quickmacros.com
 Homepage=www.quickmacros.com
 Category=Operating Systems or Utilities
 Description=Automates repetitive tasks.
 
 [License]
 Shareable=true
 OpenSource=false
 Freeware=false
 CommercialUse=true
 
 [Control]
 Start=qm_pe.exe
 
s1.setfile(_s.from(sfappinfo "\appinfo.ini"))

 copy icon
cop- "$qm$\qm.ico" _s.from(sfappinfo "\appicon.ico")

 copy optional files
if(d.e6fil.len) cop- d.e6fil sfmyqm

 copy qml
s1.from(sfmyqm "\Main.qml")
if(dir(s1))
	if(silent) goto g2
	OsdHide
	sel mes("Do you want to keep your macro list file that is in the removable drive (%s)? If you click No, it will be replaced with your current file." mtit "YN?" s1)
		case 'Y' goto g2
		case 'N' del s1; err
cop curfile s1
 g2

 finished
if(!silent)
	OsdHide
	mes "Finished." mtit "i"

err+ mes- _error.description mtit "x"

 notes:
 Paths in dat will be changed by qm_pe.exe. Because need unexpanded paths that now are unknown.


 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 290 112 "Portable QM Setup"
 3 Static 0x54000000 0x0 4 6 42 12 "Destination"
 4 Edit 0x54030080 0x200 48 4 200 14 "dri"
 8 Button 0x54032000 0x0 250 4 18 14 "..."
 7 Button 0x54032000 0x0 270 4 18 14 "?"
 5 Static 0x54000000 0x0 4 28 42 22 "Add files[](optional)"
 6 Edit 0x54231044 0x200 48 26 200 56 "fil"
 11 Button 0x54032000 0x0 250 26 18 14 "..."
 9 Button 0x54032000 0x0 270 26 18 14 "?"
 1 Button 0x54030001 0x4 90 94 48 14 "OK"
 2 Button 0x54030000 0x4 142 94 48 14 "Cancel"
 12 Static 0x54000010 0x20000 4 88 281 1 ""
 END DIALOG
 DIALOG EDITOR: "QP_DIALOG" 0x2030109 "*" "" ""

ret
 messages
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 8 if(BrowseForFolder(s1)) s1.setwintext(id(4 hDlg))
	case 11
	s2="$my qm$"; ARRAY(str) af
	if(OpenSaveDialog(0 0 "" "" s2 "" 0 af))
		s1.getwintext(id(6 hDlg))
		s2=af; s2.rtrim("[]"); s1.addline(s2)
		s1.setwintext(id(6 hDlg))
	case 7 mes "USB or other removable drive (for example E), or an existing folder in it. QM files will be stored there, in folder 'QuickMacrosPortable'.[][]To add QM to the PortableApps.com menu, specify PortableApps folder in the drive (for example E:\PortableApps)." "" "i"
	case 9 mes "The macro copies whole QM program folder ($qm$) to the drive. However it does not copy your whole QM data folder ($my qm$). To the data folder it adds only your current macro list file and files and/or folders specified here. Use *? to match multiple files (but not folders)." "" "i"
ret 1
