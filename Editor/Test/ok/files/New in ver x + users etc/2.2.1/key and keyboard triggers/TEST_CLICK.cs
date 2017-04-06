function [$wintext]
if(!len(wintext)) ret
int w1=win(wintext "" "" findc(wintext '*')>=0)
out

spe 100

 act w1
 ret

lef 90 250 w1
lef+ 100 250 w1
lef- 200 250 w1

str s.getsel
out s
mou
