
 Ends current internet function (downloading, connecting, etc). Also disconnects, if connected.
 Must be called from other thread than the thread where the function is running.
 The function will fail, unless already completed.
 This function is asynchronous.
 This function can be used, for example, to implement timeout, "Cancel" button, or to end thread.

 Added in: QM 2.3.2.


if m_hi ;;info: m_hi==0 in GetUrl and Ftp.Connect.
	Ftp* f; Http* h
	if(m_isFtp) f=&this; f.Disconnect
	else h=&this; h.Disconnect
else
	InternetCloseHandle(m_hitop); m_hitop=0
	Init
