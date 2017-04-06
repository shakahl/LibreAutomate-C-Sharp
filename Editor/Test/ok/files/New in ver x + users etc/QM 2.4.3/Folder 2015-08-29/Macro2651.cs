int w=CreateWindowEx(0 "MyClass1" 0 WS_VISIBLE|WS_SYSMENU 0 0 600 600 0 0 _hinst 0)
opt waitmsg 1
wait 0 -WC w
