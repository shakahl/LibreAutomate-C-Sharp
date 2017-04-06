function $serverName cbFunc [cbParam]

Stop

m_cbFunc=cbFunc
m_cbParam=cbParam

if(DdeInitializeW(&m_idinst &__DdeServer_Callback CBF_SKIP_REGISTRATIONS|CBF_SKIP_UNREGISTRATIONS 0)) end ES_FAILED

#compile "Dde"
 in QM 2.3.2 __DdeStr renamed to ___DdeStr
#ifdef ___DdeStr
___DdeStr sn
#else
__DdeStr sn
#endif
if(!DdeNameService(m_idinst sn.Create(serverName m_idinst) 0 DNS_REGISTER)) Stop; end ES_FAILED
