def RAS_MaxEntryName 256
def RAS_MaxDeviceType 16
def RAS_MaxDeviceName 128
def RAS_MaxPhoneNumber 128
def RAS_MaxCallbackNumber RAS_MaxPhoneNumber
def RASBASE 600
def ERROR_BUFFER_TOO_SMALL (RASBASE+3)

def RASCS_OpenPort 0
def RASCS_PortOpened 1
def RASCS_ConnectDevice 2
def RASCS_DeviceConnected 3
def RASCS_AllDevicesConnected 4
def RASCS_Authenticate 5
def RASCS_AuthNotify 6
def RASCS_AuthRetry 7
def RASCS_AuthCallback 8
def RASCS_AuthChangePassword 9
def RASCS_AuthProject 10
def RASCS_AuthLinkSpeed 11
def RASCS_AuthAck 12
def RASCS_ReAuthenticate 13
def RASCS_Authenticated 14
def RASCS_PrepareForCallback 15
def RASCS_WaitForModemReset 16
def RASCS_WaitForCallback 17
def RASCS_Projected 18
def RASCS_StartAuthentication 19
def RASCS_CallbackComplete 20
def RASCS_LogonNetwork 21
def RASCS_SubEntryConnected 22
def RASCS_SubEntryDisconnected 23
def RASCS_Interactive 0x1000
def RASCS_RetryAuthentication 0x1001
def RASCS_CallbackSetByCaller 0x1002
def RASCS_PasswordExpired 0x1003
def RASCS_InvokeEapUI 0x1004
def RASCS_Connected 0x2000
def RASCS_Disconnected 0x2001

def RASCN_Connection 0x1
def RASCN_Disconnection 0x2
def RASCN_BandwidthAdded 0x4
def RASCN_BandwidthRemoved 0x8

type RASDIALPARAMS dwSize !szEntryName[RAS_MaxEntryName + 1] !szPhoneNumber[RAS_MaxPhoneNumber + 1] !szCallbackNumber[RAS_MaxCallbackNumber + 1] !szUserName[257] !szPassword[257] !szDomain[16]
type RASCREDENTIALS dwSize dwMask !szUserName[257] !szPassword[257] !szDomain[16]
type RASCONN dwSize hrasconn !szEntryName[RAS_MaxEntryName + 1] !szDeviceType[RAS_MaxDeviceType + 1] !szDeviceName[RAS_MaxDeviceName + 1] !szPhonebook[MAX_PATH] dwSubEntry
type RASENTRYNAME dwSize !szEntryName[RAS_MaxEntryName + 1]
type RASCONNSTATUS dwSize rasconnstate dwError !szDeviceType[RAS_MaxDeviceType + 1] !szDeviceName[RAS_MaxDeviceName + 1]

#opt err 1
dll- rasapi32
	#RasDial !*lpRasDialExtensions $lpszPhonebook RASDIALPARAMS*lpRasDialParams dwNotifierType !*lpvNotifier *lphRasConn
	#RasHangUp hrasconn
	#RasEnumConnections RASCONN*lprasconn *lpcb *lpcConnections
	#RasEnumEntries $reserved $lpszPhonebook RASENTRYNAME*lprasentryname *lpcb *lpcEntries
	#RasConnectionNotification hrasconn hEvent dwFlags
	#RasGetConnectStatus hrasconn RASCONNSTATUS*lprasconnstatus
	#RasGetErrorString uErrorValue $lpszErrorString cBufSize
	#RasGetCredentials $lpszPhonebook $lpszEntry RASCREDENTIALS*lpCredentials
