str s="net get file[]net send file"
ARRAY(str) a=s
int i
for i 0 a.len
	int r=NetSendMacro(ip "p" a[i] "\Fffolder")
	if(r) ErrMsg 1 r
	
