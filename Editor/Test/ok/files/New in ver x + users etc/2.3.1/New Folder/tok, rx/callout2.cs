 /
function[c]# CALLOUT*p
 out p.start_match
 out p.current_position

out "param=%i pos=%i" p.frx.paramc p.current_position
 out p.frx
 ARRAY(str)& a=+p.frx.paramc
 out a.len
