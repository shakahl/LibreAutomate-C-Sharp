run "$documents$\Knygos\Feeling Good\David_D._Burns__Feeling_Good_The_New_Mood_Therapy.pdf"
int w=wait(30 WA win("David_D._Burns__Feeling_Good_The_New_Mood_Therapy.pdf - Foxit Reader - [David_D._Burns__Feeling_Good_The_New_Mood_Therapy.pdf]" "classFoxitReader"))
0.5
 key C2
key A{vps} ;;single page view
Acc a.Find(w "OUTLINEITEM" "now" "class=SysTreeView32[]id=19501" 0x1005 5)
a.Mouse(1 -10 5); mou

run "$documents$\Knygos\Feeling Good\Feeling Good LT.doc"
int w1=wait(30 WA win("Feeling Good LT.doc - Microsoft Word" "OpusApp"))
0.5
key CE
key CS2 ;;keyboard LT

 RestoreMultiWinPos("feeling good")
mov 0 0 w; siz 655 1.0 w 4
mov 653 0 w1; siz ScreenWidth-655 1.0 w1 4
