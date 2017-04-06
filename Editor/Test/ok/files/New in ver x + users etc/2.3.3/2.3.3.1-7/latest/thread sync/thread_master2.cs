 start 3 threads and get thread handles
ARRAY(int) ath
ath[]=mac("thread_slave")
ath[]=mac("thread_slave")
ath[]=mac("thread_slave")

wait 3

 end all threads
int i
for i 0 ath.len
	if WaitForSingleObject(ath[i] 0)=WAIT_TIMEOUT
		shutdown -6 0 "" ath[i]
