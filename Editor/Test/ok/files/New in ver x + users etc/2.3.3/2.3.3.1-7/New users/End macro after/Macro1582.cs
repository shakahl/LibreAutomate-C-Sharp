 add this at the beginning of the macro
int t1=GetTickCount
int t2=6000*60*60 ;;6 hours

rep
	 add this at the beginning of the loop
	if(GetTickCount-t1>=t2) ret ;;end this macro if 6 hours passed
	 then your code
	1
