 DOES NOT WORK. Not tested with dialog, but in VB also does not work.
typelib NetConnect {955E547D-2DE1-4704-B343-CDEDC21F2566} 1.0
opt waitmsg 1
NetConnect.NetConnect c._create
 c.AutoConnection
c.DefaultConnection
10
 BSTR src("http://www.quickmacros.com") dest(_s.expandpath("$desktop$\qm.htm"))
 c.DownloadURL(&src &dest)
 c.Connected
 NetConnect.__NetConnect e

 out _hresult
