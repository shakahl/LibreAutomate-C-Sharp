rep 300
	mac "sub.Thread"
mes "Wait" "Wait!!"


#sub Thread
int k=512*1024
if(!SetThreadStackGuarantee(&k)) end "error"
1
wait 0 -WC "Wait!!"
