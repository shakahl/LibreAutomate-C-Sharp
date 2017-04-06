function [flags] ;;flags: 1 clear results

 Ends text capturing (removes hooks etc). Optional.

 flags:
   1 - free the results array. After this, it cannot be used.

 REMARKS
 Optional. Called implicitly when destroying the variable.
 After calling End() you can still use the results array (created by Capture() or other text capturing function) until the variable is destroyed or a text capturing function called.
 It's not error to call this function more than 1 time.


if(!m_tc) ret
m_tc.End
if(flags&1) a=0; n=0; m_captured=0; m_tc=0

 undocumented:
 flag 1 also allows you to correctly unload qmtc32.dll if you want. Without this, if you unload the dll while this variable still exists, will be exception and memory leak.
