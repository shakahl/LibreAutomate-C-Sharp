out
int h1=mac("mailsot1")
int h2=mac("mailsot2")
wait 0 HM h1 h2


__Handle wt=CreateWaitableTimer(0 0 "Global\QM test timer")
long t=-1
SetWaitableTimer(wt +&t 0 0 0 0)
