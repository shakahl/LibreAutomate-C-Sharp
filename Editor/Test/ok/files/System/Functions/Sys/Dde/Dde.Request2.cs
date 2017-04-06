function# $item str&data [^timeout]

 Retrieves a string.
 Stores it into str variable data.
 Note: Not all programs support Unicode.


if(!m_conv) end ERR_INIT
if(!item or !&data) end ERR_BADARG
if(timeout<=0) timeout=60
___DdeStr hs1
int cf=CF_UNICODETEXT
 g1
int hd=DdeClientTransaction(0 0 m_conv hs1.Create(item m_idinst) cf XTYP_REQUEST timeout*1000 0)
if(hd)
	lpstr s=DdeAccessData(hd 0)
	if(cf=CF_UNICODETEXT) data.ansi(s); else data=s
	DdeUnaccessData(hd)
	DdeFreeDataHandle(hd)
	ret 1
else if(cf=CF_UNICODETEXT and DdeGetLastError(m_idinst)=DMLERR_NOTPROCESSED)
	 some programs don't support Unicode
	cf=CF_TEXT; goto g1
