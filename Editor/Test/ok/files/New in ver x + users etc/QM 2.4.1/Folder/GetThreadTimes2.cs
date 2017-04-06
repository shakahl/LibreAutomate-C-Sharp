out
int h=mac("Function277")
long tc te tk tu ptk ptu
int i
for i 0 100
	if(!GetThreadTimes(h +&tc +&te +&tk +&tu)) break
	if(i) out F"{tk-ptk/10} {tu-ptu/10}"
	ptk=tk; ptu=tu
	if(WaitForSingleObject(h 1000)!=WAIT_TIMEOUT) break

