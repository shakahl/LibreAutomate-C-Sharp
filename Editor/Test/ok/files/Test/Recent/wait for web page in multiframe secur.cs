 with http://www.fineco.it/

act "Maxthon"
10 P
men 32826 "Maxthon" ;;Clean Cache
wait 0 P
5
int ie=win(" Internet Explorer")
act ie
0.5

Htm el=htm("TD" "Prestiti e mutui" "" ie "2" 67 0x21)
el.Click
spe
 web "http://msdn.microsoft.com/library/" ;;frame local
 web "http://www.live365.com/cgi-bin/directory.cgi?genre=Presets" ;;iframe remote
 web "http://www.m-w.com/" ;;iframe local
 web "http://www.download.com/" 0 ie
 web "http://www.quickmacros.com/" 0 ie

wait 0 I
 wait 0 S "Macro89.bmp" "Fineco" 0 0x1

mes "ok"
key AL
