#compile ctoqm_dll
out

IExprC e=CreateExpr

lpstr s
s="1+-4"
s="3"
s="012"
s="0xffffffffffffffff"
 s="1+2*3+-((4<<3)/5)" ;;1


int i n=1000
long r
Q &q
rep n
	r=1+2
Q &qq
rep n
	r=e.EvalC(s)
Q &qqq
outq
out r
