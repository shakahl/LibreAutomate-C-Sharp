#compile ctoqm_dll
out

IExprC e=CreateExpr
IStringMap dm=CreateStringMap(0); e.SetDefMap(dm); dm.Add("DEF" "2")
 ref mup "mup_api"
 int h=mup.mupInit; atend mup.mupRelease h

 lpstr s="1+2"
lpstr s="1+DEF"
 lpstr s="3"
 lpstr s="1+2*3+-(4<<3/5)"
int i r n=1000

Q &q
rep n
	r=1+2
Q &qq
rep n
	r=e.EvalC(s)
Q &qqq
 rep n
	 mup.mupSetExpr(h s)
	 r=mup.mupEval(h)
 Q &qqqq
outq
 out r
