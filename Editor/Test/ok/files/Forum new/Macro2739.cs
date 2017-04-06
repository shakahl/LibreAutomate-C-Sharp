 deb 2000
run "Q:\Downloads\iMindQ_15day_7_0_1_51051_Sites.exe"
int w=wait(30 WA win("iMindQ® - InstallShield Wizard" "MsiDialogCloseClass"))

#region Recorded 2016-12-27 08:46:46
 int w1=act(win("iMindQ® - InstallShield Wizard" "MsiDialogCloseClass")) ;;;;;;;;the same as above
lef 351 333 w 1 ;;push button 'Next >'
wait 0 -WV w
int w2=wait(15 win("iMindQ® - InstallShield Wizard" "MsiDialogCloseClass"))
lef 36 263 w2 1 ;;radio button 'I accept the terms in the l...'
lef 349 334 w2 1 ;;push button 'Next >'
wait 0 -WV w2

 it seems that your installer does not have this page, therefore your macro would not have these 4 lines
int w3=wait(24 win("iMindQ® - InstallShield Wizard" "MsiDialogCloseClass"))
lef 34 254 w3 1 ;;check box 'I would like to receive inf...'
lef 349 336 w3 1 ;;push button 'Next >'
wait 0 -WV w3

int w4=wait(21 win("iMindQ® - InstallShield Wizard" "MsiDialogNoCloseClass"))
lef 72 258 w4 1 ;;check box 'FreeMind (.mm) files'
lef 70 280 w4 1 ;;check box 'MindJet MindManager (.mmap)...'
lef 344 335 w4 1 ;;push button 'Next >'
#endregion
