Acc+ g_accTest
int+ g_accMarshal
if !g_accTest.a
	int w=win("This PC" "CabinetWClass")
	g_accTest.Find(w "LISTITEM" "Downloads" "class=DirectUIHWND" 0x1005)
	
	if(CoMarshalInterThreadInterfaceInStream(IID_IAccessible el +&g_accMarshal)) end ERR_FAILED

	mes 1

 out g_accTest.Name
 Error (RT) in <open ":1: /153">Macro2781:  0x8001010E, The application called an interface that was marshalled for a different thread.    <help #IDP_ERR>?

