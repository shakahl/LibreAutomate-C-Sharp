out

ARRAY(WINAPI2.IP_ADAPTER_INFO) AdapterInfo.create(16)
WINAPI2.IP_ADAPTER_INFO* pAdapterInfo = &AdapterInfo[0]
int dwBufLen = AdapterInfo.len*sizeof(WINAPI2.IP_ADAPTER_INFO)

int dwStatus = WINAPI2.GetAdaptersInfo(pAdapterInfo &dwBufLen)
if(dwStatus) end "failed"

lpstr adapterName IP description
str MAC
rep
	adapterName=&pAdapterInfo.AdapterName
	out F"adapterName: {adapterName}"
	
	description=&pAdapterInfo.Description
	out F"description: {description}"
	
	
	MAC.fromn(&pAdapterInfo.Address pAdapterInfo.AddressLength)
	MAC.encrypt(8)
	out F"MAC: {MAC}"
	
	IP=&pAdapterInfo.IpAddressList.IpAddress
	out F"IP: {IP}"
	
	out "----------"
	
	pAdapterInfo = pAdapterInfo.Next
	if(!pAdapterInfo) break
