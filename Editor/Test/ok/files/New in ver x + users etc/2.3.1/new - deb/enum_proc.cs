 /Macro1126
function h p

 end "error"

 spe 500
 wait -2

zw h
 ret 1

 opt slowmouse -1
 out getopt(slowmouse)
 out getopt(slowmouse 1)
 out getopt(slowmouse 2)
 out getopt(slowmouse 3)
 spe 100
 mou 10 10

 opt slowmouse 1
 out spe(-1)
 out spe
 spe -1
 out spe
 out spe(0)
 out spe(-2)
 mou 10 10
