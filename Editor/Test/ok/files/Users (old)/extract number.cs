CURRENCY a b="100000"
str c="AMOUNT (100,000.00)"

if(findrx(c "\d+(,\d+)*\.\d\d" 0 2 c)<0) out "number not found"; ret
out c
a=c

mes iif(a<=b "ok" "not ok")
