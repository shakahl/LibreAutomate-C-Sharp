 out
 int w=GetTopWindow(0)
 rep
	 w=RealGetNextWindow(w 0)
	 if(!w) break
	 outw2 w

out
outw GetTopWindow(0)
outw GetWindow(0 GW_HWNDFIRST)
