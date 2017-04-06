 /
function save str*controls $regkey [$only]

 Saves or initializes dialog controls.
 Useful only for edit, check and radio.

 save - if 1, save controls (use after ShowDialog). If 0, gets saved controls (use before ShowDialog).

if(only and !only[0]) only=0
int i
ARRAY(str) a
tok controls[0] a
for i 0 a.len
	if(only and findw(only a[i])<0) continue
	if(save) rset controls[i+1] a[i] regkey
	else rget controls[i+1] a[i] regkey
