ClearOutput
int r=NetSendMacro(ip "p" "net_Macro")
out r
if(r) ret

str sr
r=net(ip "p" "net_Macro" sr 5)
out r
out sr
