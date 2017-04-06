 \
int hwnd=val(_command)
int i h m s; str ss
for i 0 1000000000
	if(!IsWindow(hwnd)) ret
	ss.getwintext(hwnd)
	if(i) ss.replacerx(" <\d+:\d+:\d+>$") ;;remove previous timer
	s=i%60
	m=i/60%60
	h=i/3600
	ss.formata(" <%i:%02i:%02i>" h m s)
	ss.setwintext(hwnd)
	1
err+
