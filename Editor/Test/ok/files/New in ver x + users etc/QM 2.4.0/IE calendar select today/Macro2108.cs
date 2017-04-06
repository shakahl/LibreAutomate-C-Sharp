web "https://co​nnect.jlre​xt.com/dan​a-na/auth/​url_9/welc​ome.cgi" 1
int w=act(win("Dealer SSL VPN - Windows Internet Explorer" "IEFrame"))
AutoPassword "m-farook" "mohaf2013" 5 w

 #ret
 ---- Recorded 2013.09.08 09:50:53 ----
opt slowkeys 1; opt slowmouse 1; spe 100
int w1=act(win("Dealer access SSL VPN - Home - Windows Internet Explorer" "IEFrame"))
err
	mes "waiting for confirmation"
	w1=win("Dealer access SSL VPN - Home - Windows Internet Explorer" "IEFrame")
lef 193 331 w1 1 ;;Internet Explorer_Server 'https://connect.jlrext.com/...', link 'JLR - Vista'
int w2=wait(5 win("VISTA - Windows Internet Explorer" "IEFrame"))
err
	w=wait(30 WA win("Web Single Login - Windows Internet Explorer" "IEFrame"))
	AutoPassword "m-farook" "mohaf2013" 5 w
	w2=wait(15 win("VISTA - Windows Internet Explorer" "IEFrame"))
lef 80 103 w2 1 ;;Internet Explorer_Server 'https://connect.jlrext.com/...', link 'Order'
lef 102 159 w2 1 ;;Internet Explorer_Server 'https://connect.jlrext.com/...', link 'Find Orders'
wait 30 WT w2 "Find Orders - Windows Internet Explorer"
max w2
lef+ 174 238 w2 1 ;;Internet Explorer_Server 'https://connect.jlrext.com/...'
int w3=win("" "Internet Explorer_Server")
lef-;; 156 194 w3 1
lef 107 195 w3 1 ;;list item 'VIN'
lef 305 240 w2 1 ;;Internet Explorer_Server 'https://connect.jlrext.com/...', editable text
'"SALGA2DF3EA130581"
lef 440 951 w2 1 ;;Internet Explorer_Server 'https://connect.jlrext.com/...', editable text 'Search'
wait 30 WT w2 "View Order - Windows Internet Explorer"
lef 1593 952 w2 1 ;;Internet Explorer_Server 'https://connect.jlrext.com/...', editable text 'Maintain Order'
wait 30 WT w2 "Maintain Order - Windows Internet Explorer"
lef 50 617 w2 1 ;;Internet Explorer_Server 'https://connect.jlrext.com/...', editable text 'Handover'
opt slowkeys 0; opt slowmouse 0; spe -1
 --------------------------------------


