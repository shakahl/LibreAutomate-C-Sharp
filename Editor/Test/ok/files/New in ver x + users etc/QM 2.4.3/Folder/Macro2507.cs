 out
 SendMail "support@quickmacros.com" "test" "k" 0 "" "" "" "" "" "gmail"
 outw2 win
 int w=win("Email account" "#32770")
 int w=win("QM Email Settings" "#32770")
 w=_hwndqm
 outw GetLastActivePopup(w)
 outw GetWindow(w GW_ENABLEDPOPUP)
 outw GetAncestor(w 3)

 int w1=win("Dialog1" "#32770")
 int w2=win("Dialog2" "#32770")
 int w3=win("Dialog3" "#32770")
 int w4=win("Dialog4" "#32770")
 outw2 GetLastActivePopup(w1)
 outw2 GetLastActivePopup(w2)
 outw2 GetLastActivePopup(w3)
 outw2 GetLastActivePopup(w4)
 out ""
 outw2 GetWindow(w1 GW_ENABLEDPOPUP)
 outw2 GetWindow(w2 GW_ENABLEDPOPUP)
 outw2 GetWindow(w3 GW_ENABLEDPOPUP)
 outw2 GetWindow(w4 GW_ENABLEDPOPUP)

 men 33552 _hwndqm
