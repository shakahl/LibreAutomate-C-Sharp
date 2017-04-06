ExcelSheet k.Init
 k.ws._setevents("sub")
k.ws._setevents("sub.ko")
opt waitmsg 1
10

 ExcelSheet esk.Init
 esk.ws._setevents("esk_DocEvents")
 opt waitmsg 1
 10

 sub.setevents_from_sub

#sub k_SelectionChange
function Excel.Range'Target ;;Excel._Worksheet'es
out 1

#sub ko_SelectionChange
function Excel.Range'Target ;;Excel._Workbook'es
out 2

#sub _SelectionChange
function Excel.Range'Target ;;Excel._Worksheet'es
out 3

#sub setevents_from_sub
 ExcelSheet es.Init
 es.ws._setevents("es_DocEvents")
 opt waitmsg 1
 10

ExcelSheet k.Init
k.ws._setevents("sub")
opt waitmsg 1
10

#sub subErr
ssss
