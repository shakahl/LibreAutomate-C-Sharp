 act
 ----
 int w=wait(10 WV win("LHMT _ Orų prognozė - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
 Acc a.FindFF(w "A" "Skaitmeninė orų prognozė" "" 0x1001 10)
 a.DoDefaultAction
  ----
 int w1=wait(10 WV win("LHMT _ Skaitmeninė orų prognozė - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
 Acc a1.FindFF(w1 "A" "" "href=skaitmenine_prog_lt_miest.php" 0x1004 10)
 a1.DoDefaultAction

 ----
 int w2=wait(10 WV win("LHMT _ Skaitmeninė orų prognozė - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
 act w2
 Acc a2.FindFF(w2 "SELECT" "" "name=skpm_miestas" 0x1004 10)
 a2.CbSelect("Rokiškis")

 ----
 int w=wait(10 WV win("" "Mozilla*WindowClass" "" 0x804))
 Acc a.FindFF(w "SELECT" "" "name=lr" 0x1004 10)
 a.CbSelect("italų")
 ----
 int w1=wait(10 WV win("" "Mozilla*WindowClass" "" 0x804))
 Acc a1.FindFF(w1 "SELECT" "" "name=num" 0x1004 10)
 a1.CbSelect("50 *")

 ----
int w=wait(10 WV win("Pagrindinė paieška - Pažintys - Draugas.lt - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
 act w
Acc af.FindFF(w "FORM" "" "name=npaieska" 0x1004 10)

Q &q
Acc a.FindFF(af.a "SELECT" "" "name=miestas" 0x1004 10)
a.CbSelect("Rok*" 0)
 ----
Acc a1.FindFF(af.a "SELECT" "" "name=amzius" 0x1004 10)
a1.CbSelect("30*" 0)
 ----
Acc a2.FindFF(af.a "SELECT" "" "name=amzius2" 0x1004 10)
a2.CbSelect("40*" 0)
 ----
Acc a3.FindFF(af.a "SELECT" "" "name=lytis" 0x1004 10)
a3.CbSelect("M*" 0)
 ----
Q &qq
outq
