int hwnd=ShowDialog("" 0 0 0 1)

str localfile="$desktop$\quickm20.gi_"
IntGetFile "http://www.quickmacros.com/images/quickm20.gif" localfile 16 0 0 &IntGetFile_progress_callback hwnd
err ret ;;error or Cancel
ren- localfile "$desktop$\quickm20.gif"

clo hwnd

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 222 42 "Form"
 2 Button 0x54030000 0x4 164 24 48 14 "Cancel"
 3 msctls_progress32 0x54000000 0x0 8 6 204 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""
