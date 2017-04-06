function# $item $data [datalen] [^timeout] ;;for strings, datalen should be omitted or <=0

 Sends a string.


if(!m_conv) end ERR_INIT
if(!item or !data) end ERR_BADARG
int cf=CF_TEXT
if(datalen<1) data=_s.unicode(data); datalen=_s.len+2; cf=CF_UNICODETEXT
if(timeout<=0) timeout=60
___DdeStr hs1
ret DdeClientTransaction(data datalen m_conv hs1.Create(item m_idinst) cf XTYP_POKE timeout*1000 0)
