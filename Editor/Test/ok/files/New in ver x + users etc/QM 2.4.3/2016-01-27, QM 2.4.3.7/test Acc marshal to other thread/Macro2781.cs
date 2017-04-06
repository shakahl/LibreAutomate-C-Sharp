Acc a
int+ g_accMarshal
int w=win("This PC" "CabinetWClass")
a.Find(w "LISTITEM" "Downloads" "class=DirectUIHWND" 0x1005)

if(CoMarshalInterThreadInterfaceInStream(IID_IAccessible a.a +&g_accMarshal)) end ERR_FAILED

mes 1

 out g_accTest.Name
 Error (RT) in <open ":1: /153">Macro2781:  0x8001010E, The application called an interface that was marshalled for a different thread.    <help #IDP_ERR>?

