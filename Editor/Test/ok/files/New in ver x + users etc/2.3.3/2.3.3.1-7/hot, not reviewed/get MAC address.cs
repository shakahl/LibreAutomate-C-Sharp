str computerName="" ;;this computer
 str computerName="ComputerName"
 str computerName="www.quickmacros.com" ;;this does not work for me, always gets 00 1B 21 75 AB 5A, or fails on other computer, maybe it is because of firewall

dll iphlpapi #SendARP DestIP SrcIP !*pMacAddr *PhyAddrLen

str ipSrc ipDst
 if(!GetIpAddress("" &ipSrc)) end "failed"
if(!GetIpAddress(computerName &ipDst)) end "unknown computer"
 out ipSrc
 out ipDst

str MAC.all(8); _i=8
 int e=SendARP(inet_addr(ipDst) inet_addr(ipSrc) MAC &_i)
int e=SendARP(inet_addr(ipDst) 0 MAC &_i)
if(e) end _s.dllerror("failed." "" e)
MAC.fix(_i)
MAC.encrypt(8 MAC "" 1)
out MAC
