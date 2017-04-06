#if 0
def CCHMAX_FOLDER_NAME 256
def CLSID_StoreNamespace uuidof("{E70C92A9-4BFD-11D1-8A95-00C04FB951F3}")
def CMF_DELETE 0x0002
def CMF_MOVE 0x0001
def COMMITSTREAM_REVERT 0x00000001
def FOLDERID_INVALID 0xffffffff
def FOLDERID_ROOT 0
type FOLDERPROPS cbSize dwFolderId cSubFolders sfType cUnread cMessage !szName[256]
def FOLDER_DELETED 3
def FOLDER_DRAFT 4
def FOLDER_INBOX 0
def FOLDER_MAX 5
def FOLDER_NOTSPECIAL 0xFFFFFFFF
def FOLDER_OUTBOX 1
def FOLDER_SENT 2
type HBATCHLOCK = HBATCHLOCK__*
type HBATCHLOCK__ unused
type HENUMSTORE = HENUMSTORE__*
def IID_IStoreFolder uuidof("{E70C92AC-4BFD-11D1-8A95-00C04FB951F3}")
def IID_IStoreNamespace uuidof("{E70C92AA-4BFD-11D1-8A95-00C04FB951F3}")
type IMSGPRIORITY = #
def IMSG_PRI_HIGH 1
def IMSG_PRI_LOW 5
def IMSG_PRI_NORMAL 3
interface# IStoreFolder :IUnknown
	GetFolderProps(dwReserved FOLDERPROPS*pProps)
	GetMessageProps(dwMessageId dwFlags MESSAGEPROPS*pProps)
	FreeMessageProps(MESSAGEPROPS*pProps)
	DeleteMessages(MESSAGEIDLIST*pMsgIdList dwReserved IProgressNotify'pProgress)
	SetLanguage(dwLanguage dwReserved MESSAGEIDLIST*pMsgIdList)
	MarkMessagesAsRead(fRead dwReserved MESSAGEIDLIST*pMsgIdList)
	SetFlags(MESSAGEIDLIST*pMsgIdList dwState dwStatemask *prgdwNewFlags)
	OpenMessage(dwMessageId GUID*riid !**ppvObject)
	SaveMessage(GUID*riid !*pvObject dwMsgFlags *pdwMessageId)
	BatchLock(dwReserved HBATCHLOCK__**phBatchLock)
	BatchFlush(dwReserved HBATCHLOCK__*hBatchLock)
	BatchUnlock(dwReserved HBATCHLOCK__*hBatchLock)
	CreateStream(HBATCHLOCK__*hBatchLock dwReserved IStream*ppStream *pdwMessageId)
	CommitStream(HBATCHLOCK__*hBatchLock dwFlags dwMsgFlags IStream'pStream dwMessageId IStream'pMessage)
	RegisterNotification(dwReserved hwnd)
	UnregisterNotification(dwReserved hwnd)
	Compact(dwReserved)
	GetFirstMessage(dwFlags dwMsgFlags dwMsgIdFirst MESSAGEPROPS*pProps !**phEnum)
	GetNextMessage(!*hEnum dwFlags MESSAGEPROPS*pProps)
	GetMessageClose(!*hEnum)
	{E70C92AC-4BFD-11d1-8A95-00C04FB951F3}
interface# IStoreNamespace :IUnknown
	Initialize(hwndOwner dwReserved)
	GetDirectory($pszPath cchMaxPath)
	OpenSpecialFolder(sfType dwReserved IStoreFolder*ppFolder)
	OpenFolder(dwFolderId dwReserved IStoreFolder*ppFolder)
	CreateFolder(dwParentId $pszName dwReserved *pdwFolderId)
	RenameFolder(dwFolderId dwReserved $pszNewName)
	MoveFolder(dwFolderId dwParentId dwReserved)
	DeleteFolder(dwFolderId dwReserved)
	GetFolderProps(dwFolderId dwReserved FOLDERPROPS*pProps)
	CopyMoveMessages(IStoreFolder'pSource IStoreFolder'pDest MESSAGEIDLIST*pMsgIdList dwFlags dwFlagsRemove IProgressNotify'pProgress)
	RegisterNotification(dwReserved hwnd)
	UnregisterNotification(dwReserved hwnd)
	CompactAll(dwReserved)
	GetFirstSubFolder(dwFolderId FOLDERPROPS*pProps !**phEnum)
	GetNextSubFolder(!*hEnum FOLDERPROPS*pProps)
	GetSubFolderClose(!*hEnum)
	{E70C92AA-4BFD-11d1-8A95-00C04FB951F3}
type LPFOLDERNOTIFYEX = tagFOLDERNOTIFYEX*
type LPFOLDERPROPS = FOLDERPROPS*
type LPHBATCHLOCK = HBATCHLOCK__**
type LPHENUMSTORE = HENUMSTORE__**
type LPMESSAGEID = #*
type LPMESSAGEIDLIST = MESSAGEIDLIST*
type LPMESSAGEPROPS = MESSAGEPROPS*
type LPSTOREFOLDER = IStoreFolder
type LPSTOREFOLDERID = #*
type LPSTORENAMESPACE = IStoreNamespace
type MESSAGEID = #
type MESSAGEIDLIST cbSize cMsgs *prgdwMsgId
def MESSAGEID_FIRST 0xffffffff
def MESSAGEID_INVALID 0xffffffff
type MESSAGEPROPS cbSize dwReserved dwMessageId dwLanguage dwState cbMessage priority FILETIME'ftReceived FILETIME'ftSent $pszSubject $pszDisplayTo $pszDisplayFrom $pszNormalSubject dwFlags IStream'pStmOffsetTable
def MSGPROPS_FAST 0x00000001
def MSG_DELETED 0x0001
def MSG_EXTERNAL_FLAGS 0x00FE
def MSG_FLAGS 0x000f
def MSG_LAST 0x0080
def MSG_NEWSMSG 0x0020
def MSG_NOSECUI 0x0040
def MSG_RECEIVED 0x0010
def MSG_SUBMITTED 0x0004
def MSG_UNREAD 0x0002
def MSG_UNSENT 0x0008
def MSG_VOICEMAIL 0x0080
type SPECIALFOLDER = #
type STOREFOLDERID = #
