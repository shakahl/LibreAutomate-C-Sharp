 Used as a local variable, to automatically reset (set = 0) a global or thread int variable when the function exits.
 Reliably resets, regardless of how the function returns - normally or because of an error or calling end or when the user ends the thread (except when shows warning "thread terminated").
 Usually you just call Init(), and don't use Reset(). If need, you can call Init() and Reset() multiple times.

 EXAMPLES

int+ g_resetTest
sub.Function
out g_resetTest


#sub Function
#compile "__AutoResetVariable"
int+ g_resetTest
AutoResetVariable x.Init(g_resetTest 1)
out g_resetTest
mes "function running"
