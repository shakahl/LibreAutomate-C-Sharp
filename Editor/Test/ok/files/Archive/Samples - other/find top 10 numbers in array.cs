out

 create array for testing

ARRAY(double) a.create(20)
int i
for(i 0 a.len) a[i]=RandomNumber*200 ;;generate random numbers between 0 and 200

 see what we have
out "--- Array ---"
for(i 0 a.len) out a[i]
 ______________________________

 find 10 biggest numbers

ARRAY(int) ai ;;will contain indices of max 10 biggest numbers in a

ai.create(iif(a.len>=10 10 a.len))
ARRAY(byte) at.create(a.len)
int j k
for(j 0 ai.len)
	double mx=-1; int imx=0
	for(k 0 a.len) if(!at[k] and a[k]>mx) mx=a[k]; imx=k
	ai[j]=imx
	at[imx]=1
 ______________________________

 results

out "--- Top %i ---" ai.len
for(i 0 ai.len) out "%i, value=%f" ai[i] a[ai[i]]
