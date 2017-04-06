 (Vista+)
out
ref WINAPI2
INetworkListManager x._create(CLSID_NetworkListManager)
word is
 x.GetConnectivity(_i); out _i
 x.GetNetwork
 x.GetNetworkConnection
 x.get_IsConnected(is); out is
 x.get_IsConnectedToInternet(is); out is
 IEnumNetworkConnections e; x.GetNetworkConnections(&e)
 INetworkConnection c
 foreach c e
	 c.get_IsConnectedToInternet(is)
	 out is

IEnumNetworks e
x.GetNetworks(NLM_ENUM_NETWORK_ALL e)
INetwork n
foreach n e
	BSTR b; b.free
	n.GetName(b)
	out b
	GUID g; n.GetNetworkId(g); out _s.FromGUID(g)

    <NetworkSettings>
      <Id>{FA68CC0F-7A5E-4E32-A8F9-4CEFDA8CDF6F}</Id>
      <Name>Network  2</Name>
    </NetworkSettings>
