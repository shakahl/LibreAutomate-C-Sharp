 int w=win("Calculator" "CalcFrame")
 Acc a.Find(w "PUSHBUTTON" "8" "class=Button[]id=138" 0x1005)
 Acc a.Find("Calculator" "PUSHBUTTON" "8" "class=Button[]id=138" 0x1005)

 Acc aw.Find(w "DIALOG" "" "class=#32770" 0x1004 0 2)
 Acc a.Find(aw "PUSHBUTTON" "8" "" 0x1005)
 Acc a.Find(aw.a "PUSHBUTTON" "8" "" 0x1005)

 VARIANT v=w
 v=aw.a
 Acc a.Find(v "PUSHBUTTON" "8" "class=Button[]id=138" 0x1005)

 int w=wait(3 WV win("LHMT _ Skaitmeninė orų prognozė - Mozilla Firefox" "MozillaWindowClass"))
 FFNode f.FindFF(w "UL" "<li><a href=''index.php''>Pirmasis puslapis</a></li><li><a href=''naujienos.php''>Naujienos</a></li><li><a href=''struktura.php''>Struktūra ir kontaktai</a></li><li><a href=''teisine_inf.php''>Teisinė informacija</a></li><li><a href=''veikla.php''>Veikla</a></li><li><a class=''m_aktyvus'' href=''paslaugos.php''>Paslaugos</a></li><li class=''subpunktas''><a href=''oru_prognoze.php''>Orų prognozė</a></li><li class=''subpunktas''><a class=''m_aktyvus'' href=''skaitmenine_prog.php''>Skaitmeninė orų prognozė</a></li><li class=''subpunktas''><a href=''sgrips.php''>Pavojingi reiškiniai</a></li><li class=''subpunktas''><a href=''faktiniai_orai.php''>Faktiniai orai</a></li><li class=''subpunktas''><a href=''radaro_inf.php''>Radaro informacija</a></li><li class=''subpunktas''><a href=''palydovine_inf.php''>Palydovinė informacija</a></li><li class=''subpunktas''><a href=''zaibu_islydziai.php''>Žaibų išlydžiai</a></li><li class=''subpunktas''><a href=''hidro_informacija.php''>Hidrologinė informacija</a></li><li class=''subpunktas''><a href=''misku_gaisr.php''>Miškų gaisringumas</a></li><li><a href=''stebejimu_tinklas.php''>Stebėjimų tinklas</a></li><li><a href=''klimatas.php''>Klimatas</a></li><li><a href=''apzvalgos.php''>Apžvalgos</a></li><li><a href=''skaiciuokles.php''>Skaičiuoklės</a></li><li><a href=''ivairenybes.php''>Įvairenybės</a></li><li><a href=''literatura.php''>Literatūra</a></li><li><a href=''klausimai.php''>Klausimai</a></li><li><a href=''korupcijos_prevencija.php''>Korupcijos prevencija</a></li><li><a href=''nuorodos.php''>Nuorodos</a></li>" "" 0x1001 3)
 Acc a.FindFF(f "A" "Veikla" "" 0x1001 3)
  Acc a.FindFF(f.node "A" "Veikla" "" 0x1001 3)







 a.DoDefaultAction
 a.Mouse
