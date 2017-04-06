ref MSOEAPI "msoeapi"
class COeMessages MSOEAPI.IStoreNamespace'ns --nothing
type OEFOLDER folderId parentId cSubFolders sfType cUnread cMessage str'name
type OEMESSAGE dwMessageId dwLanguage dwState cbMessage priority FILETIME'ftReceived FILETIME'ftSent ~pszSubject ~pszDisplayTo ~pszDisplayFrom ~pszNormalSubject dwFlags ~source
