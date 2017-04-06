#compile ctoqm_dll
out

IExprC e=CreateExpr
IStringMap dm=CreateStringMap(0)
e.SetIdMap(dm)
e.SetIdFunc(&CH_UnkId22 &dm)

lpstr s
s="1+2"
s="1+DEF*2"
s="1+UNK*2"
s="1+defined(DEF)*2"
s="1+defined(UNK)*2"
s="1+Function40(1, 2)*2"
 s="'A'"
 s="'abc'"
 s="L'A'"
 s="sizeof(''abc'')"
 s="sizeof(''\x30'')"
 s="sizeof(L''abc'')"
 s="sizeof(L''\x30'')"
s="sizeof(''\'''')"

dm.Add("DEF" "5*3")

int i=e.EvalC(s)
out i
 out "%s" &i
