BSTR b.alloc(1000)
int n=b.len

 these get all caps, same as GetComputerName that is used by QM functions
 if(!GetComputerNameExW(ComputerNameNetBIOS b &n)) end "failed"
 if(!GetComputerNameExW(ComputerNamePhysicalNetBIOS b &n)) end "failed"

 these get correct case
 if(!GetComputerNameExW(ComputerNameDnsHostname b &n)) end "failed"
 if(!GetComputerNameExW(ComputerNameDnsFullyQualified b &n)) end "failed" ;;this can get HostName.DomainName
if(!GetComputerNameExW(ComputerNamePhysicalDnsHostname b &n)) end "failed"
 if(!GetComputerNameExW(ComputerNamePhysicalDnsFullyQualified b &n)) end "failed"

str s=b
out s

 Don't know which is the best. I would choose the non-commented. But look in GetComputerNameEx documentation in MSDN.
 With xxxPhysicalxxx always get local computer name. About others MSDN says:
 "If the local computer is a node in a cluster, gets the DNS host name of the cluster virtual server."
