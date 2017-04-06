function str&so

so=":: "
byte* b
rep
	for(b pidl b+pidl.cb) so.formata("%02X" b[0])
	if(!b[0] and !b[1]) break
	pidl=b; so+" "
