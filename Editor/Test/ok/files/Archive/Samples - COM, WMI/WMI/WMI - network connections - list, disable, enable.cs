 note: run as administrator.

out

def wbemFlagReturnImmediately 16
def wbemFlagForwardOnly 32

IDispatch wmi._getfile("winmgmts:")
IDispatch col = wmi.ExecQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL" @ wbemFlagReturnImmediately|wbemFlagForwardOnly)

IDispatch x
foreach x col
	 out x.NetConnectionId
	str name=x.Name
	out name
	if name="Realtek PCIe GBE Family Controller"
		x.Disable
		mes "Now should be disabled. Click OK to enable again."
		x.Enable
		 break
