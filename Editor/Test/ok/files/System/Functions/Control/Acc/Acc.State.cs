function# [str&statetext]

 Gets state.
 Returns <google "site:microsoft.com object state constants IAccessible">state flags</google>.

 statetext - variable that receives state text.

 Tip: You can see object states in dialog "Find accessible object. Click the Properties button. Or in the "Other properties" grid click the state value and click the small button.

 EXAMPLES
 str state
 a.State(state)
 out state

 if(a.State&STATE_SYSTEM_READONLY) out "readonly"


if(!a) end ERR_INIT
VARIANT v=a.State(elem); err end _error
if(&statetext)
	int i(1) st=v.lVal
	if(st)
		statetext.fix(0)
		BSTR b.alloc(100)
		rep 32
			if(st&i)
				GetStateTextW(i b 100)
				if(statetext.len) _s.ansi(b); statetext.from(statetext ", " _s)
				else statetext.ansi(b)
			i<<1
	else statetext="normal"
ret v.lVal
