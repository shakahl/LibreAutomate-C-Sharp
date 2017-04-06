 ---- Recorded 7/8/2012 10:04:09 PM ----
int w1=act(win("Microsoft Excel - MergeMe0708" "XLMAIN"))
'Cc; 1.25 ;; Ctrl+C
int w2=act(win("MRI.GOT (G/LIVE.MIS/529/GMH) - SCRIPT - \\Remote" "Transparent Windows Client"))
'Av; 1.56 ;; Alt+V
act w1
'R Cc; 1.37 ;; Right Ctrl+C
act w2
'Av; 3.59 ;; Alt+V
'Y; 0.70 ;; Enter
'"y" Y; 0.57 ;; "y" Enter	2
4 win("Confirmation - \\Remote" "Transparent Windows Client")
'"y"; 2.73 ;; "y"
3
rep 100
	act w1
	'DL Cc; 1.00 ;; Down Left Ctrl+C
	act w2
	'Av; 1.59 ;; Alt+V
	act w1
	'R Cc; 1.68 ;; Right Ctrl+C
	act w2
	'Av; 1.91 ;; Alt+V
	'Y; 0.74 ;; Enter
	'"y" Y; 0.42 ;; "y" Enter
	2
	3 win("Confirmation - \\Remote" "Transparent Windows Client")
	'"y" ;; "y"
 ---------------------------------------
