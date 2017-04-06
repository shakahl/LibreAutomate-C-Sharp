str sf="$my qm$\test\ok.db3"
str sf2="G:\ok.db3"
 str sf2="\\GINTARAS\MyProjects\test\ok.db3"

 PF
 str s.getfile(sf)
 PN
 s.setfile(sf2) ;;1.63, 0.85
 PN
 PO
 #ret

PF
Sqlite x.Open(sf2)
PN
x.Exec("BEGIN TRANSACTION")
 x.Exec("UPDATE items SET name='vvvvvvvvvvvvvvvvvvvvvvvv' WHERE id==1000")
 x.Exec("UPDATE items SET name='vvvvvvvvvvvvvvvvvvvvvvvv' WHERE id==1")
 x.Exec("UPDATE items SET name='vvvvvvvvvvvvvvvvvvvvvvvv' WHERE id==2")
x.Exec("UPDATE texts SET text='vvvvvvvvvvvvvvvvvvvvvvvv' WHERE id==1")
 x.Exec("UPDATE texts SET text='vvvvvvvvvvvvvvvvvvvvvvvv' WHERE id==2")
x.Exec("END TRANSACTION")
PN
x.Close
PN
PO

 usb:
 speed: 4094  149479  123895  

 network:
 speed: 19333  147025  1958  
 speed: 9462  19603  1863  (cached)

 vmware:

