function [$appID]

 Begins building jump list for your application.
 Call this function once, before all other functions.

 appID - see <google>ICustomDestinationList::SetAppID</google>. Optional.


if(m_list) end ERR_BADARG

m_list._create(WINAPI7.CLSID_DestinationList)
if(!empty(appID)) m_list.SetAppID(@appID)
IUnknown u
m_list.BeginList(&_i uuidof(IUnknown) &u)
m_col._create(WINAPI7.CLSID_EnumerableObjectCollection)

err+ end _error
