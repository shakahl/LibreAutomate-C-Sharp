sub.macro "640" "635"
sub.macro "720" "715"
 and so on


#sub macro
function str'param1 str'param2

spe 100
#region Recorded 12/22/2014 12:39:44 PM
int w1=act(win("UVProbe - [Spectrum]" "Afx:*" "" 0x4))
lef+ 568 23 w1 1 ;;tool bar 'Spectrum Toolbar', push button 'Method (Ctrl+M)'
lef- 558 21 w1 1
int w2=wait(5 win("Spectrum Method" "#32770"))
lef 245 58 w2 1 ;;editable text 'Start:'
lef+ 253 45 w2 1 ;;editable text 'Start:'
lef- 148 49 w2 1

key (param1) ;;was '"640". Now we use a variable instead.

lef+ 380 50 w2 1 ;;editable text 'to  End:'
lef- 312 50 w2 1

key (param2) ;;was '"635". Now we use a variable instead.

lef 367 384 w2 1 ;;push button 'OK'
act w1
lef 350 890 w1 1 ;;tool bar 'Photometer Buttons', push button 'Start Acquisition (F9)'
lef 1863 30 win("" "Shell_TrayWnd") 1 ;;clock 'Clock'
int w3=wait(65 WV win("New Data Set" "#32770"))
wait 10
lef 189 243 w3 1 ;;push button 'OK'
wait 5
act w1
lef 43 -5 w1 1 ;;menu item 'File'
lef 57 63 wait(5 WV win("" "#32768")) 1 ;;menu item 'Save As...'
 men 57604 w1 ;;
int w4=wait(5 win("Save Spectrum File" "#32770"))
lef 220 608 w4 1 ;;editable text 'File name:'
lef+ 220 608 w4 1 ;;editable text 'File name:'
lef- 694 874 w1 1
'"1"            ;; "1"
lef 904 640 w4 1 ;;combo box 'Save as type:', push button 'Close'
lef 383 44 win("" "ComboLBox") 1 ;;list item 'Peak Pick Table (*.txt)'
lef 953 611 w4 1 ;;push button 'Save'
wait 5
#endregion
