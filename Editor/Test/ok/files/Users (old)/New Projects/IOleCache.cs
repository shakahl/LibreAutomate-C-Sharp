out
def GUID_NULL uuidof("{00000000-0000-0000-0000-000000000000}")
IOleCache ic
if(CreateDataCache(0 GUID_NULL IID_IOleCache &ic)) ret
out 1

IDataObject do
if(OleGetClipboard(&do)) ret
out 2

ic.InitCache(do); err ret
out 3

IEnumSTATDATA esd
ic.EnumCache(&esd); err ret
out 4

STATDATA sd; int n
esd.Reset
esd.Next(1 &sd &n)
out n ;;0

 How to retrieve IDataObject from the cache?
