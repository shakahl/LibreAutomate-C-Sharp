
int+ fnMessageBoxA=&MessageBox
int+ fnMessageBoxW=&MessageBoxW

if(!ApiSetHook(+&fnMessageBoxA &MyMessageBoxA)) end "failed to hook"
 if(!ApiSetHook(+&fnMessageBoxW &MyMessageBoxW)) end "failed to hook"

MessageBox(0 "text" "cap" 0)
 MessageBoxW(0 @"text" @"cap" 0)

if(!ApiUnhook(+&fnMessageBoxA)) end "failed to unhook"
 if(!ApiUnhook(+&fnMessageBoxW)) end "failed to unhook"
