typelib WMPLib {6BF52A50-394A-11D3-B153-00C04F79FAA6} 1.0
type PLA_ITEM ~path
type PLA_DATA hdlg hlv iPlaying WMPLib.WindowsMediaPlayer'p ARRAY(PLA_ITEM)a
PLA_DATA- d

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 355 274 "Simple MP3 Player"
 12 Edit 0x54030080 0x200 30 4 222 16 ""
 4 Static 0x54000000 0x0 2 6 24 10 "Folder"
 5 Button 0x54032000 0x0 256 6 48 14 "Browse..."
 3 SysListView32 0x54030000 0x200 0 68 356 206 ""
 6 Button 0x54032000 0x0 6 46 38 14 "<<"
 7 Button 0x54032000 0x0 6 30 38 14 "Play"
 9 Button 0x54032000 0x0 46 30 38 14 "Stop"
 10 Button 0x54032000 0x0 46 46 38 14 ">>"
 11 Button 0x54032000 0x0 306 6 48 14 "Refresh"
 8 ActiveX 0x54030000 0x0 208 28 144 37 "WMPLib.WindowsMediaPlayer {6BF52A52-394A-11D3-B153-00C04F79FAA6}"
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

str controls = "12"
str e12
if(!ShowDialog(dd &PLA_Dialog &controls)) ret
