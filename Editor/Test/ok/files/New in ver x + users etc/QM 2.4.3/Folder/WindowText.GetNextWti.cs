function'WTI* WTI*t indexDiff

 Gets another text item.
 Returns address of internal variable that contains item properties.

 t - a text item. Usually it is the value returned by Find.
 indexDiff - index of item to get minus index of item t.
   For example, if index of t is 5, and indexDiff is 1, this function gets item 6. If indexDiff is -1, gets item 4.
   To see item indices, in dialog "Window Text" click Test next to Select.

 EXAMPLE
 int w=win("Calculator" "CalcFrame")
 WindowText wt.Init(w)
 WTI* t=wt.Find("5" 0x1001)
 t=wt.GetNextWti(t 1)
 wt.Mouse(0 t)


t+indexDiff*sizeof(WTI)
if(t<a or t>=a+(n*sizeof(WTI))) end ERR_BADARG
ret t
