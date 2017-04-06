 /
 Each mouse-triggered macro that has this filter function,
 will run only if mouse pointer is over the clock.

function# iid FILTER&f

str s.getwinclass(WindowFromPoint(f.x f.y))
if(s~"TrayClockWClass") ret iid
