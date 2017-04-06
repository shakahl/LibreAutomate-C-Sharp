 /Macro1352
function $HTML

if(!d)
	d._create(uuidof(MSHTML.HTMLDocument))
	d3=+d

interface# __IPersist :IUnknown
	GetClassID(GUID*pClassID)
	{0000010c-0000-0000-C000-000000000046}
interface# __IPersistStreamInit :__IPersist
	IsDirty()
	Load(IStream'pStm)
	Save(IStream'pStm fClearDirty)
	GetSizeMax(ULARGE_INTEGER*pCbSize)
	InitNew()
	{7FD52380-4E07-101B-AE2D-08002B2EC713}

__IPersistStreamInit ps=+d
__Stream t.CreateOnHglobal(HTML len(HTML)+1)
ps.InitNew
ps.Load(t)

opt waitmsg 1
int i ii
for(i 1 500) ;;wait max 125s
	wait i/1000.0
	sel(d.readyState 1)
		case "complete" break
		case "interactive" ii+1; if(ii>50) break
if(i=500) end "timeout"

 int i
for i 0 d.all.length
	MSHTML.IHTMLElement el=d.all.item(i)
	out el.tagName

err+ end _error
