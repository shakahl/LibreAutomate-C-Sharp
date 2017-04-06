 #compile mup_api
ref mup "mup_api"

int h=mup.mupInit
atend mup.mupRelease h
 mup.mupDefineOprtChars(h "")
 mup.mupDefineInfixOprtChars(h "")

IStringMap m=CreateStringMap(0)

int i
str s
lpstr cn; double cv
Q &q
for i 0 1000
	s.from("CONSTANT" i)
	mup.mupDefineConst(h s i)
	 mup.mupGetConst(h i &cn &cv)
	 m.Add(s s)
	 cn=m.Get(s)
Q &qq
outq

 mup.mupSetErrorHandler

 mup.mupSetExpr(h "1+(2)")
 int i=mup.mupEval(h)
if(mup.mupError)
	 out mup.mupGetErrorCode
	out mup.mupGetErrorMsg
 out i
