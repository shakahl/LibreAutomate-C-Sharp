
 Unregisters window class registered by Register.

 REMARKS
 Optional. Called implicitly when destroying variable.
 Fails if a window of the class exists.
 If fails, does not throw error, but displays warning in QM output.


if atom
	if(!UnregisterClassW(+atom _hinst)) end F"failed to unregister window class. {_s.dllerror}" 8
	atom=0
	baseClassWndProc=0
	baseClassCbWndExtra=0
if(m_hicon) DestroyIcon(m_hicon); m_hicon=0
if(m_hiconSm) DestroyIcon(m_hiconSm); m_hiconSm=0
