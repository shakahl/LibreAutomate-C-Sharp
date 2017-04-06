#opt hidedecl 1
interface# IServiceProvider :IUnknown
	QueryService(GUID*guidService GUID*riid !**ppvObject)
	{6d5140c1-7436-11ce-8034-00aa006009fa}
interface# IDataObject :IUnknown
	GetData(FORMATETC*pformatetcIn STGMEDIUM*pmedium)
	GetDataHere(FORMATETC*pformatetc STGMEDIUM*pmedium)
	QueryGetData(FORMATETC*pformatetc)
	GetCanonicalFormatEtc(FORMATETC*pformatectIn FORMATETC*pformatetcOut)
	SetData(FORMATETC*pformatetc STGMEDIUM*pmedium fRelease)
	EnumFormatEtc(dwDirection IEnumFORMATETC*ppenumFormatEtc)
	DAdvise(FORMATETC*pformatetc advf IAdviseSink'pAdvSink *pdwConnection)
	DUnadvise(dwConnection)
	EnumDAdvise(IEnumSTATDATA*ppenumAdvise)
	{0000010e-0000-0000-C000-000000000046}
interface# ISequentialStream :IUnknown
	Read(!*pv cb *pcbRead)
	Write(!*pv cb *pcbWritten)
	{0c733a30-2a1c-11ce-ade5-00aa0044773d}
interface# IStream :ISequentialStream
	Seek(LARGE_INTEGER'dlibMove dwOrigin ULARGE_INTEGER*plibNewPosition)
	SetSize(ULARGE_INTEGER'libNewSize)
	CopyTo(IStream'pstm ULARGE_INTEGER'cb ULARGE_INTEGER*pcbRead ULARGE_INTEGER*pcbWritten)
	Commit(grfCommitFlags)
	Revert()
	LockRegion(ULARGE_INTEGER'libOffset ULARGE_INTEGER'cb dwLockType)
	UnlockRegion(ULARGE_INTEGER'libOffset ULARGE_INTEGER'cb dwLockType)
	Stat(STATSTG*pstatstg grfStatFlag)
	Clone(IStream'ppstm)
	{0000000c-0000-0000-C000-000000000046}
interface# IOleCommandTarget :IUnknown
	QueryStatus(GUID*pguidCmdGroup cCmds OLECMD*prgCmds OLECMDTEXT*pCmdText)
	Exec(GUID*pguidCmdGroup nCmdID nCmdexecopt VARIANT*pvaIn VARIANT*pvaOut)
	{b722bccb-4e68-101b-a2bc-00aa00404770}
interface# IOleWindow :IUnknown
	GetWindow(*phwnd)
	ContextSensitiveHelp(fEnterMode)
	{00000114-0000-0000-C000-000000000046}
interface# IEnumUnknown :IUnknown
	Next(celt IUnknown*rgelt *pceltFetched)
	Skip(celt)
	Reset()
	Clone(IEnumUnknown*ppenum)
	{00000100-0000-0000-C000-000000000046}
interface# IParseDisplayName :IUnknown
	ParseDisplayName(IBindCtx'pbc @*pszDisplayName *pchEaten IMoniker*ppmkOut)
	{0000011a-0000-0000-C000-000000000046}
interface# IOleContainer :IParseDisplayName
	EnumObjects(grfFlags IEnumUnknown*ppenum)
	LockContainer(fLock)
	{0000011b-0000-0000-C000-000000000046}
interface# IPicture :IUnknown
	get_Handle(*pHandle)
	get_hPal(*phPal)
	get_Type(@*pType)
	get_Width(*pWidth)
	get_Height(*pHeight)
	Render(hDC x y cx cy xSrc ySrc cxSrc cySrc RECT*pRcWBounds)
	set_hPal(hPal)
	get_CurDC(*phDC)
	SelectPicture(hDCIn *phDCOut *phBmpOut)
	get_KeepOriginalFormat(*pKeep)
	put_KeepOriginalFormat(keep)
	PictureChanged()
	SaveAsFile(IStream'pStream fSaveMemCopy *pCbSize)
	get_Attributes(*pDwAttr)
	{7BF80980-BF32-101A-8BBB-00AA00300CAB}
#opt hidedecl 0
