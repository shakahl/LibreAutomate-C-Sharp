ExcelSheet esk.Init
esk.ws._setevents("sub.esk")

#sub esk_Calculate
function ;;Excel._Worksheet'esk


#sub esk_BeforeDoubleClick
function Excel.Range'Target @&Cancel ;;Excel._Worksheet'esk


#sub esk_Deactivate
function ;;Excel._Worksheet'esk


#sub esk_Activate
function ;;Excel._Worksheet'esk


#sub esk_Change
function Excel.Range'Target ;;Excel._Worksheet'esk


#sub esk_SelectionChange
function Excel.Range'Target ;;Excel._Worksheet'esk


#sub esk_FollowHyperlink
function Excel.Hyperlink'Target ;;Excel._Worksheet'esk
