/
function [$regcode] [$email] [$orderid]


str controls = "5 7 9 11"
str e5nam e7ema e9ord e11reg

if(!empty(regcode)) e5nam.get(regcode 33); e5nam.escape(8)
e7ema=email
if(empty(orderid)) e9ord.Guid; else e9ord=orderid ;;cannot be empty, although unused; must be unique value
e11reg=regcode

if(!ShowDialog("" 0 &controls)) ret

ARRAY(POSTFIELD) a.create(4)
a[0].name="name"; a[0].value=e5nam
a[1].name="email"; a[1].value=e7ema
a[2].name="orderid"; a[2].value=e9ord
a[3].name="regcode"; a[3].value=e11reg

Http h.Connect("www.quickmacros.com")
h.PostFormData("reg/rcadd.php" a _s)

_s.findreplace("<br>" "[]")
mes _s "Response" "i"

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 433 122 "Dialog"
 1 Button 0x54030001 0x4 152 104 48 14 "OK"
 2 Button 0x54030000 0x4 202 104 48 14 "Cancel"
 3 Static 0x54000000 0x0 2 4 218 10 "This will be added to the online database of QM customers:"
 4 Static 0x54000000 0x0 4 28 38 12 "Name"
 5 Edit 0x54030080 0x200 44 26 386 14 "nam"
 6 Static 0x54000000 0x0 4 44 38 12 "Email"
 7 Edit 0x54030080 0x200 44 42 386 14 "ema"
 8 Static 0x54000000 0x0 4 60 38 12 "Orderid"
 9 Edit 0x54030080 0x200 44 58 386 14 "ord"
 10 Static 0x54000000 0x0 4 76 38 12 "Regcode"
 11 Edit 0x54030080 0x200 44 74 386 14 "reg"
 END DIALOG
 DIALOG EDITOR: "" 0x2030106 "" "" ""
