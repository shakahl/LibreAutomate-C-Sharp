 Before running this macro, move mouse on program name

Acc a a2
str name price size date dl dllw

a=acc(mouse)
name=a.Name

a.Navigate("parent2") ;;cell 1

a.Navigate("last" a2)
size=a2.Name

a2.Navigate("prev2")
price=a2.Name

a.Navigate("next") ;;cell 2

a.Navigate("first" a2)
date=a2.Name

a.Navigate("next") ;;cell 3

a.Navigate("first" a2)
dl=a2.Name

a.Navigate("last prev" a2)
dllw=a2.Name

name.replacerx(" [\d.]+$")
price.replacerx(".+?([\d\.]+).+" "$1" 4)
int i=findc(price '.'); if(i>0) if(val(price+i+1)>=50) price=val(price)+1; else price=val(price)

 mes- "Name: %s[]Price: %s[]DL: %s[]DLLV: %s[]Date: %s[]Size: %s" "" "OC" name price dl dllw date size

act "Excel"
 wait 0 ML
key (name) T (price) T (dl) T (dllw) T (date) T (size) HD
1.5
act
