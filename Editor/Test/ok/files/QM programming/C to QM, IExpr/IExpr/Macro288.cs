#compile ctoqm_dll
out

out(0xffffffff>1);
out(-1U>1);
out(((0xffffffffU||0U)*(-1))>1);
out(((0xffffffffU>0U)*(-1))>1);
out((1U*(-1))>1);

out "----"

IExprC e=CreateExpr
out e.EvalC("0xffffffff>1");
out e.EvalC("-1U>1");
 out e.EvalC("-1U");
out e.EvalC("((0xffffffffU||0U)*(-1))>1");
out e.EvalC("((0xffffffffU>0U)*(-1))>1");
out e.EvalC("(1U*(-1))>1");
