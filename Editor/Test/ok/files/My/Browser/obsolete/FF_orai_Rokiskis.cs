 out _s.timeformat
spe -1
int hwnd=val(_command)
 ----
 0.5
Acc a.FindFF(hwnd "A" "" "href=skaitmenine_prog_lt_miest.php" 0x1004 10)
a.DoDefaultAction
FirefoxWait
 ----
 0.5
int w1=wait(10 WV win("LHMT _ Skaitmeninė orų prognozė - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
 Acc a1.FindFF(w1 "SELECT" "" "name=skpm_miestas" 0x1004 10)
Acc a1.FindFF(w1 "SELECT" "Vilnius" "name=skpm_miestas" 0x1004 10)
a1.CbSelect("Rokiškis")

err+
