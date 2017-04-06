ClearOutput
str sr s.getfile("$qm$\winapiqm.txt")
int r=net(ip "p" "net_send_file" sr s)
out r
out sr
