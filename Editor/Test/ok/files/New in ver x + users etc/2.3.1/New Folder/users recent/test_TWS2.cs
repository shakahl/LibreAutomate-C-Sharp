out
 #opt dispatch 1

typelib TWSLib {0A77CCF5-052C-11D6-B0EC-00B0D074179C} 1.0

str HostIP="" ;;leave blank if TWS runs on the same computer
int Port=7496
int ClientID=0

 OleUninitialize
 CoInitializeEx 0 COINIT_MULTITHREADED

 opt waitmsg 1

TWSLib.Tws t._create
 t._setevents("t__DTwsEvents")
 TWSLib.Tws t._create("TWS.TwsCtrl.1")

 IDispatch t._create(uuidof(TWSLib.Tws))

 TWSLib.Tws t
 out CoCreateInstance(uuidof(TWSLib.Tws) 0 CLSCTX_SERVER uuidof(TWSLib._DTws) &t)
 out t

t.connect("", 7496, 0)
 1
