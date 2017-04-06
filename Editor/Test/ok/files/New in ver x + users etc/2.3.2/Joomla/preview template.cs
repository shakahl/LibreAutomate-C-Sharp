 In Firefox, move mouse over zip and press Ctrl+t.

str tf="$temp$\templates"
mkdir tf

Acc a=acc(mouse)
str url=a.Value

str fn.getfilename(url 1)
str tfz.from(tf "\" fn)

IntGetFile url tfz 16
zip- tfz tf
del- tfz

tfz.fix(tfz.len-4)
run tfz
