 /
function str'chart int&cloaked
if(!&cloaked) out "bad argument 'cloaked', must be an int variable"
int w=win(chart)
if(!w) end "window not found"

out "activating and uncloaking"

 tryagain
0.01
act w
err
	out "cannot activate window in actuncloak"
	goto tryagain
	
if win<>w or win=0
	out "trying to activate window in actuncloak again"
	goto tryagain

cloaked=(IsWindowCloaked(w))
int active(win=w) visible(IsWindowVisible(w))
out "active=%i visible=%i cloaked=%i" active visible cloaked
if cloaked
	cloaked=0
	if(DwmSetWindowAttribute(w 13 &cloaked 4))
		out _s.dllerror ;;Try to uncloak. Error: Access is denied..
		ret

ret