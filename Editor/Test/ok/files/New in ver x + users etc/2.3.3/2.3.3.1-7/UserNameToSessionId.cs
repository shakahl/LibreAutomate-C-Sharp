 /Macro1614
function# $user

 enum sessions
WTS_SESSION_INFO* psi; int i n found
if(!WTSEnumerateSessions(0 0 1 &psi &n)) ret -1
for i 0 n
	WTS_SESSION_INFO& r=psi[i]
	 if(!StrCompare(r.pWinStationName "Console" 1))
	 out r.pWinStationName
	 out r.SessionId
	 out r.State
	word* s
	if(!WTSQuerySessionInformationW(0 r.SessionId WTSUserName &s &_i)) continue
	_s.ansi(s)
	 out _s
	WTSFreeMemory s
	if(_s~user) ret r.SessionId

WTSFreeMemory psi
