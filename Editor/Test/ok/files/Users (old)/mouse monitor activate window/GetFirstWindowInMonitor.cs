 /
function# h mon

 Simplified.

rep
	h=GetWindow(h iif(h GW_HWNDNEXT GW_HWNDFIRST)); if(!h) break
	if(MonitorFromWindow(h MONITOR_DEFAULTTONULL)!=mon) continue
	if(!IsWindowVisible(h) or !IsWindowEnabled(h)) continue
	if(GetWindow(h GW_OWNER) or GetWinStyle(h 1)&WS_EX_TOOLWINDOW) continue
	ret h
