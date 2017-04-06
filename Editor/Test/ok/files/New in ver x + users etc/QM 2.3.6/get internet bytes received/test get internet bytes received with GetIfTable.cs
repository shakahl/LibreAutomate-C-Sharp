type MIB_IFROW @wszName[256] dwIndex dwType dwMtu dwSpeed dwPhysAddrLen !bPhysAddr[8] dwAdminStatus dwOperStatus dwLastChange dwInOctets dwInUcastPkts dwInNUcastPkts dwInDiscards dwInErrors dwInUnknownProtos dwOutOctets dwOutUcastPkts dwOutNUcastPkts dwOutDiscards dwOutErrors dwOutQLen dwDescrLen !bDescr[256]
type MIB_IFTABLE dwNumEntries MIB_IFROW'table[1]
dll iphlpapi #GetIfTable MIB_IFTABLE*pIfTable *pdwSize bOrder

str sb
int i n=100*sizeof(MIB_IFTABLE)
rep
	MIB_IFTABLE* t=sb.all((n+10)*sizeof(MIB_IFTABLE))
	int hr=GetIfTable(t &n 0); if(!hr) break
	if(hr!=ERROR_INSUFFICIENT_BUFFER) end _s.dllerror("" "" hr)

out
for i 0 t.dwNumEntries
	MIB_IFROW& r=t.table[i]
	int kbReceived=r.dwInOctets/1024
	if(kbReceived=0) continue
	out F"{kbReceived} KB received and {r.dwOutOctets/1024} KB sent by {&r.bDescr%%s} ({&r.wszName%%S})"
