out
ICsv c=CreateCsv

c.Separator=","
str s=
 0,   0xffff
 0xffff,   0xff00, five, six, seven, eith, ten
 
c.FromString(s)

int hwnd=id(1580 win("Options" "#32770"))
c.ToQmGrid(hwnd 2)
