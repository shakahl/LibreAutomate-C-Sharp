#ret
def MAPI_LOGON_UI  0x00000001
def MAPI_NEW_SESSION  0x00000002
def MAPI_DIALOG  0x00000008
def MAPI_RECEIPT_REQUESTED  0x00000002

type MapiRecipDesc ulReserved ulRecipClass $lpszName $lpszAddress ulEIDSize !*lpEntryID
type MapiFileDesc ulReserved flFlags nPosition $lpszPathName $lpszFileName !*lpFileType
type MapiMessage ulReserved $lpszSubject $lpszNoteText $lpszMessageType $lpszDateReceived $lpszConversationID flFlags MapiRecipDesc*lpOriginator nRecipCount MapiRecipDesc*lpRecips nFileCount MapiFileDesc*lpFiles

dll- mapi32
	#MAPILogon ulUIParam $lpszProfileName $lpszPassword flFlags ulReserved *lplhSession
	#MAPILogoff lhSession ulUIParam flFlags ulReserved
	#MAPISendMail lhSession ulUIParam MapiMessage*lpMessage flFlags ulReserved
	#MAPISendDocuments ulUIParam $lpszDelimChar $lpszFullPaths $lpszFileNames ulReserved
	#MAPIAddress lhSession ulUIParam $lpszCaption nEditFields $lpszLabels nRecips MapiRecipDesc*lpRecips flFlags ulReserved *lpnNewRecips MapiRecipDesc**lppNewRecips
	#MAPIFreeBuffer !*pv
