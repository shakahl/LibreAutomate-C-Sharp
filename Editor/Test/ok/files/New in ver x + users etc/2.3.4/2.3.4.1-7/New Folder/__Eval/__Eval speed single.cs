 use Ctrl+R, not the Run button

int a b c d e
Q &q
a=__Eval("2|8")
Q &qq
b=__Eval("2|8")
Q &qqq
c=__Eval("WS_VISIBLE")
Q &qqqq
d=__Eval("WS_VISIBLE+100")
Q &qqqqq
e=__Eval("{ret 100}") ;;not supported
Q &qqqqqq
outq
out "%i %i %i %i %i" a b c d e
