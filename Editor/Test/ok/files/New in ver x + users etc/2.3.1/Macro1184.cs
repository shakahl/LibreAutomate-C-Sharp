 goto g1
_i=1
 MessageBox 0 "" "" 0
 mes 1
 int w3=id(2053 _hwndqm)
 if(child(mouse)=w3)
	  g1
	 act win("Quick Macros - ok - [Macro1184]" "QM_Editor")
	 'Cp
	 int w1=act(win("Properties - Macro1184" "#32770"))
	 lef+ 232 10 w1
	 lef- 1342 429
	 lef 53 377 w1
	 int w2=wait(5 win("Task Scheduler - Macro1184" "#32770"))
	 lef 223 173 w2
	 lef 233 173 w2
 else
	 mes 1


 WM_WINDOWPOSCHANGING 0 448544
 WM_WINDOWPOSCHANGED 0 448544
 WM_NCACTIVATE 0 1835826
 WM_ACTIVATE 0 1835826
