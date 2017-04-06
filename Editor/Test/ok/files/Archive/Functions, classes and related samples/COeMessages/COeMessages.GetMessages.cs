function OEFOLDER&folder ARRAY(OEMESSAGE)&a [flags] ;;flags: 1 get message source

 Gets all messages in a folder.

 folder - a folder retrieved by GetFolders or some other function.
 a - receives message info. OEMESSAGE contains the same info as MESSAGEPROPS, documented in MSDN library.


ref MSOEAPI
if(!ns) end ES_INIT
a=0
if(!folder.cMessage) ret

int h
IStoreFolder f
ns.OpenFolder(folder.folderId 0 &f); err ret ;;'unspecified error' with Junk E-mail folder
MESSAGEPROPS mp.cbSize=sizeof(mp)
f.GetFirstMessage(0 0 MESSAGEID_FIRST &mp +&h)
if(!_hresult)
	rep
		OEMESSAGE& m=a[]
		m.cbMessage=mp.cbMessage
		m.dwFlags=mp.dwFlags
		m.dwLanguage=mp.dwLanguage
		m.dwMessageId=mp.dwMessageId
		m.dwState=mp.dwState
		m.ftReceived=mp.ftReceived
		m.ftSent=mp.ftSent
		m.priority=mp.priority
		m.pszDisplayFrom=mp.pszDisplayFrom
		m.pszDisplayTo=mp.pszDisplayTo
		m.pszNormalSubject=mp.pszNormalSubject
		m.pszSubject=mp.pszSubject
		
		if(flags&1)
			IStream is=0
			f.OpenMessage(mp.dwMessageId IID_IStream &is)
			m.source.all(mp.cbMessage 2)
			is.Read(m.source m.source.len &_i)
			
		f.GetNextMessage(+h 0 &mp)
		if(_hresult) break
	f.GetMessageClose(+h)
