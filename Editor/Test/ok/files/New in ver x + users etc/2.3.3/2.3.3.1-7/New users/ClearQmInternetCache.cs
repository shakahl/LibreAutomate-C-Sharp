
 Clears cache of QM Internet functions and web browser control.
 Code from: http://support.microsoft.com/kb/815718


long groupId
int cacheEntryInfoBufferSizeInitial cacheEntryInfoBufferSize enumHandle returnValue
int* cacheEntryInfoBuffer
INTERNET_CACHE_ENTRY_INFO* internetCacheEntry

// Delete the groups first.
// Groups may not always exist on the system.
// For more information, visit the following Microsoft Web site:
// http://msdn2.microsoft.com/en-us/library/ms909365.aspx
// By default, a URL does not belong to any group. Therefore, that cache may become
// empty even when the CacheGroup APIs are not used because the existing URL does not belong to any group.

enumHandle = FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, 0, 0, &groupId, 0);

if(enumHandle)
	// If there are no items in the Cache, you are finished.
	if(ERROR_NO_MORE_ITEMS == GetLastError()) FindCloseUrlCache enumHandle; ret
	
	// Loop through Cache Group, and then delete entries.
	rep
		// Delete a particular Cache Group.
		returnValue = DeleteUrlCacheGroup(groupId, CACHEGROUP_FLAG_FLUSHURL_ONDELETE, 0);
		
		if (!returnValue && ERROR_FILE_NOT_FOUND == GetLastError())
			returnValue = FindNextUrlCacheGroup(enumHandle, &groupId, 0);
		
		if (!returnValue && (ERROR_NO_MORE_ITEMS == GetLastError() || ERROR_FILE_NOT_FOUND == GetLastError()))
			break;
	FindCloseUrlCache enumHandle

// Start to delete URLs that do not belong to any group.
enumHandle = FindFirstUrlCacheEntry(0, 0, &cacheEntryInfoBufferSizeInitial);
if(enumHandle == 0 && ERROR_NO_MORE_ITEMS == GetLastError())
	ret

cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
internetCacheEntry =  +malloc(cacheEntryInfoBufferSize);
enumHandle = FindFirstUrlCacheEntry(0, internetCacheEntry, &cacheEntryInfoBufferSizeInitial);
rep
	cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;
	 out internetCacheEntry.lpszSourceUrlName
	returnValue = DeleteUrlCacheEntry(internetCacheEntry.lpszSourceUrlName);
	
	if (!returnValue)
		returnValue = FindNextUrlCacheEntry(enumHandle, internetCacheEntry, &cacheEntryInfoBufferSizeInitial);
	
	int dwError = GetLastError();
	if (!returnValue && ERROR_NO_MORE_ITEMS == dwError)
		break;
	
	if (!returnValue && cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize)
		cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
		internetCacheEntry =  +realloc(internetCacheEntry, cacheEntryInfoBufferSize);
		returnValue = FindNextUrlCacheEntry(enumHandle, internetCacheEntry, &cacheEntryInfoBufferSizeInitial);

free(internetCacheEntry);
FindCloseUrlCache enumHandle
