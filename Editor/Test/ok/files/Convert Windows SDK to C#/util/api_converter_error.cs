 \
function $errMsg $hFile offset

if(!empty(errMsg)) out errMsg
 out hFile
 out offset

str fn.getfilename(hFile 1)
mac+ fn
err run ExeFullPath hFile; 0.5; goto g1
men 33285 id(2213 _hwndqm); 0.1 ;;Close
mac+ fn
 g1
int h=GetQmCodeEditor
act h
SendMessage h SCI.SCI_GOTOPOS offset 0
