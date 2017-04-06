 int w=win("TOOLBAR66" "QM_toolbar")
 act w
 act GetToolbarOwner(w)

 int w=win("Font" "#32770")
 int w=win("Drawing" "MsoCommandBar")
int w=win("Untitled - Paint" "MSPaintApp")
 w=id(53254 w) ;;slider 'Zoom slider'
 act w
 mou 10 10 w
 lef- 10 10 w
 lef 10 10 w
 lef- 10 10 w 2
 lef 10 10 w 2
lef 10 10 "Notepad" 1
