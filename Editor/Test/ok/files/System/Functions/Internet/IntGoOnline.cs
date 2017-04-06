 /
function# online ;;online: 0 set offline, 1 set online, 2 set online with confirmation.

 Sets online/offline state that is used by QM internet functions and Internet Explorer.
 Returns: 1 success, 0 failed or user pressed Cancel.

 REMARKS
 Some QM internet function share "offline" state with Internet Explorer. In offline state they don't work.
 This function sets online/offline state for QM internet functions and Internet Explorer.
 Functions that depend on offline state: IntGetFile (has a "set online" flag), IntCheckConnection, Http class functions, web browser control, web, HtmlDoc.InitFromWeb.
 Functions that don't depend on offline state: FTP and email functions.

 See also: <IntIsOnline>


INTERNET_CONNECTED_INFO ci
sel(online)
	case 0
	ci.dwConnectedState = INTERNET_STATE_DISCONNECTED_BY_USER
	ci.dwFlags = ISO_FORCE_DISCONNECTED
	case 1 ci.dwConnectedState = INTERNET_STATE_CONNECTED
	case 2 ret InternetGoOnline(0 0 0)
	case else end ERR_BADARG

ret InternetSetOption(0 INTERNET_OPTION_CONNECTED_STATE &ci sizeof(ci))
