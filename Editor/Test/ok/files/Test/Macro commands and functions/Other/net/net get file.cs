ClearOutput
str sr
int r=net(ip "p" "net_get_file" sr "$desktop$\led.txt")
out r
if(r) ret
ShowText "led.txt" sr
