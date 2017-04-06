out
 run "$program files$\Java\jdk1.8.0_11\demo\Stylepad\Stylepad.jar"; ret

 JabInit
 mes "JAB"; ret

 find a Java window
 int w=win("Stylepad" "SunAwtFrame")
 int w=wait(0 WV win("Stylepad" "SunAwtFrame"))
 int w=wait(0 WV win("SwingSet2" "SunAwtFrame"))
 int w=wait(0 WV win(" - OpenOffice.org" "SALFRAME")) ;;no mouse events
 outw w
 if(!w) end ERR_WINDOW

 out JAB.IsJavaWindow(w)


#compile "__JavaAO"
JavaAO x

 x.FromWindow(w)
 out x.a

 opt waitmsg 1
rep
	 out x.FromXY(xm ym)
	out x.FromXY(1423 94)
	break
	 int w=win("Stylepad" "SunAwtFrame")
	 if(w) x.FromWindow(w)
	0.5
 mes 1
