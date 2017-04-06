out
str il="$qm$\il_qm.bmp"
str csv=
F
 ,{il},{QMDD_CHECKBOXES1|QMDD_AUTOCOLORS}
 Header,,0xE
 normal,,2
 disabled,,4
 group1,0,33
 group2,,32
 group3,,32

ARRAY(byte) a
outx ShowDropdownListSimple(csv 0 a 1)
 Header,,0xE
 normal
 disabled,,4

 ,,2|8
 Header,,0xE,,,0xff
 normal
 disabled,,4,,0xff

int i; for(i 0 a.len) outx a[i]

str csv=F",,{QMDD_CHECKBOXES1|QMDD_AUTOCOLORS}[]..."
