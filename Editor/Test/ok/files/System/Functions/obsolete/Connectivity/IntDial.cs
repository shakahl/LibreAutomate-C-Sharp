function# [$connection] [nodialog]

 Dials specified or default Internet connection. If successful, returns nonzero.
 Differently than IntConnect, this function shows dialog where you can enter
 password and other info. Also, does not throw error on failure.


str s=connection; if(!s.len) RasGetDefConn s 1
IntGoOnline(1)

int conn flags
if(nodialog) flags|INTERNET_AUTODIAL_FORCE_UNATTENDED
if(InternetDialW(0 @s flags &conn 0)) ret
ret conn
