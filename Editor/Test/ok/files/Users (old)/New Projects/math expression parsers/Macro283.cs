 #compile mup_api
ref mup "mup_api"

int h=mup.mupInit
atend mup.mupRelease h
 mup.mupDefineOprtChars(h "")
 mup.mupDefineInfixOprtChars(h "")

IStringMap m=CreateStringMap(4)


int i j
str s se
Q &q
for i 0 1000
	s.from("CONSTANT" i)
	se.from("1+" s)
	mup.mupDefineConst(h s i)
	
	mup.mupSetExpr(h se)
	j=mup.mupEval(h)
	
	 s.from("1+" i)
	 mup.mupSetExpr(h s)
	 j=mup.mupEval(h)
	 m.Add(s s)
	 cn=m.Get(s)
Q &qq
outq

out j

