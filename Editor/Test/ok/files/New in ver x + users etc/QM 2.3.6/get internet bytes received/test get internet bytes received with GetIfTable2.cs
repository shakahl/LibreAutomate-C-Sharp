 type MIB_IFROW @wszName[256] dwIndex dwType dwMtu dwSpeed dwPhysAddrLen !bPhysAddr[8] dwAdminStatus dwOperStatus dwLastChange dwInOctets dwInUcastPkts dwInNUcastPkts dwInDiscards dwInErrors dwInUnknownProtos dwOutOctets dwOutUcastPkts dwOutNUcastPkts dwOutDiscards dwOutErrors dwOutQLen dwDescrLen !bDescr[256]
 type MIB_IFTABLE dwNumEntries MIB_IFROW'table[1]
 dll iphlpapi #GetIfTable MIB_IFTABLE*pIfTable *pdwSize bOrder
type NET_LUID_LH %Value []%Info
type MIB_IF_ROW2 NET_LUID_LH'InterfaceLuid InterfaceIndex GUID'InterfaceGuid @Alias[257] @Description[257] PhysicalAddressLength !PhysicalAddress[32] !PermanentPhysicalAddress[32] Mtu Type TunnelType MediaType PhysicalMediumType AccessType DirectionType !InterfaceAndOperStatusFlags OperStatus AdminStatus MediaConnectState GUID'NetworkGuid ConnectionType %TransmitLinkSpeed %ReceiveLinkSpeed %InOctets %InUcastPkts %InNUcastPkts %InDiscards %InErrors %InUnknownProtos %InUcastOctets %InMulticastOctets %InBroadcastOctets %OutOctets %OutUcastPkts %OutNUcastPkts %OutDiscards %OutErrors %OutUcastOctets %OutMulticastOctets %OutBroadcastOctets %OutQLen
type MIB_IF_TABLE2 NumEntries MIB_IF_ROW2'Table[1]
dll iphlpapi #GetIfTable2 MIB_IF_TABLE2**Table
dll iphlpapi #GetIfTable2Ex Level MIB_IF_TABLE2**Table
dll iphlpapi FreeMibTable !*Memory

MIB_IF_TABLE2* t
int hr=GetIfTable2(&t); if(hr) end _s.dllerror("" "" hr)

out
int i
for i 0 t.NumEntries
	MIB_IF_ROW2& r=t.Table[i]
	int kbReceived=r.InOctets/1024
	if(kbReceived=0) continue
	out F"{kbReceived} KB received and {r.OutOctets/1024} KB sent by {&r.Description%%S}"

FreeMibTable t
