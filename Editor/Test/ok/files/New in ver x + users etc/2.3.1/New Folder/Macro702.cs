out
Http+ g_h.Connect("www.quickmacros.com")
ARRAY(POSTFIELD) a.create(1)
a[0].name="a"; a[0].value=1
tim 5 tim_Http_PostFormData
g_h.PostFormData("form2.php" a _s)
0.01; if(!g_h.IsConnected) end "timeout"
tim 0 tim_Http_PostFormData
g_h.Disconnect
out _s
