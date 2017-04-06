 this works on Vista, however gets only adhoc networks, not all WiFi networks

out
WINAPI2.IDot11AdHocManager man._create(WINAPI2.CLSID_Dot11AdHocManager)
WINAPI2.IEnumDot11AdHocNetworks en
man.GetIEnumDot11AdHocNetworks(0 &en)

IDot11AdHocNetwork n
rep
	en.Next(1 &n &_i); err break
	 SSID
	word* w
	n.GetSSID(&w)
	str s.ansi(w)
	CoTaskMemFree w
	 signal strength
	int ssCur ssMax
	n.GetSignalQuality(&ssCur &ssMax)
	
	out "%s, %i/%i" s ssCur ssMax
