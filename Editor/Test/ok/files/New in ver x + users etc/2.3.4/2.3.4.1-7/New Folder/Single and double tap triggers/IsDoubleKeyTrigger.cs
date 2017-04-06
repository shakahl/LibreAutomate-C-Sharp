 /

 Call this at the beginning of a key-triggered macro.
 Returns 0 on single tap, 1 on double tap.
 Example macro:

 if IsDoubleKeyTrigger
 	out "double"
 else
 	out "single"

 The macro runs on double tap AND on single tap. If need OR, instead use filter function FF_KeyTriggerSingleDouble.
 In QM 2.3.3, does not work with modifier keys (Ctrt, Shift etc). It is a bug in this QM version. Works only with single key triggers.
 Does not work with mouse triggers.


int+ __IDKT_Time
int isDouble=GetTickCount-__IDKT_Time<=400
__IDKT_Time=iif(isDouble 0 GetTickCount)
ret isDouble
