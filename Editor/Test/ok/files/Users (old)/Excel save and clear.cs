str sf sf2

 assume, current file is Book1.xls on the desktop
sf.expandpath("$desktop$\Book1.xls")

act "Excel"

 save and wait for CPU idle
key Cs ;;assume, it is already saved once, and Save As dialog will not appear
10 P

 copy with unique file name
sf2=sf
UniqueFileName sf2
cop sf sf2

 select all and delete
key Ca X
