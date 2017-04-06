 /FtpAsync help
function hInternet dwContext dwInternetStatus *lpvStatusInformation dwStatusInformationLength

out "cb: %s %i" WinInetGetCallbackText(dwInternetStatus) iif(lpvStatusInformation *lpvStatusInformation 0)

sel dwInternetStatus
	case INTERNET_STATUS_HANDLE_CLOSING
	 out GetCurrentThreadId
	
	 case INTERNET_STATUS_RECEIVING_RESPONSE
	 out hInternet
	 InternetCloseHandle(hInternet)
