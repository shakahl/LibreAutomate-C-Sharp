 Turs on/off app folder sharing.

run "E:\MyProjects\app"
int w1=wait(10 WC win("app" "ExploreWClass"))
0.2
key M
wait 15 WV "+#32768"
0.2
int i
for i 0 5
	key h
	wait 2 "app Properties"; err if(i=2) ret; else continue
 key AsAw
key As
0.5
key Y
5
clo w1
