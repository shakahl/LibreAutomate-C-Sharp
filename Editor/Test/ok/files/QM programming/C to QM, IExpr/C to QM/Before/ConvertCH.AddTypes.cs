 /CtoQM
function $type_map !noDefaults

 Adds intrinsic types and some typedefs to m_mtd.

lpstr s=
 int #
 long #
 char !
 short @
 __int64 %
 __int8 !
 __int16 @
 __int32 #
 size_t #
 double ^
 bool !
 va_list $
m_mtd.AddList(s "")

if(noDefaults) goto g1

s=
 float FLOAT
 FLOAT FLOAT
 DATE DATE
 CY CURRENCY
 CURRENCY CURRENCY
 BSTR BSTR
 VARIANT VARIANT
 SAFEARRAY SAFEARRAY
 PSZ $
 PCHAR $
 LPCH $
 PCH $
 LPCCH $
 PCCH $
 NPSTR $
 LPSTR $
 PSTR $
 LPCSTR $
 PCSTR $
 PTCHAR $
 PTBYTE $
 HPSTR $
 PFORMAT_STRING $
 ULPSTR $
 LPPCSTR $*
 LPVOID !*
 LPCVOID !*
 PVOID !*
 PVOID64 !*
 ULPVOID !*
 PTR !*
 HANDLE #
 HGDIOBJ #
 HIMAGELIST #
 HTREEITEM #
 HPROPSHEETPAGE #
 I_RPC_HANDLE #
 RPC_IF_HANDLE #
 RPC_AUTH_IDENTITY_HANDLE #
 RPC_AUTHZ_HANDLE #
 I_RPC_MUTEX #
 RPC_NS_HANDLE #
 HCRYPTOIDFUNCSET #
 HCRYPTOIDFUNCADDR #
 HCRYPTMSG #
 HCERTSTORE #
 HCERTSTOREPROV #
 HCRYPTDEFAULTCONTEXT #
 NDR_CCONTEXT #
 PMIDL_XMIT_TYPE #
 RPC_SS_THREAD_HANDLE #
 HCONTEXT #
 HMETAFILEPICT #
 HWND #
 HHOOK #
 HKEY #
 HACCEL #
 HBITMAP #
 HBRUSH #
 HCOLORSPACE #
 HDC #
 HGLRC #
 HDESK #
 HENHMETAFILE #
 HFONT #
 HICON #
 HMENU #
 HMETAFILE #
 HINSTANCE #
 HPALETTE #
 HPEN #
 HRGN #
 HRSRC #
 HSTR #
 HTASK #
 HWINSTA #
 HKL #
 HMONITOR #
 HWINEVENTHOOK #
 HUMPD #
 HRAWINPUT #
 HCONVLIST #
 HCONV #
 HSZ #
 HDDEDATA #
 HDRVR #
 HWAVE #
 HWAVEIN #
 HWAVEOUT #
 HMIDI #
 HMIDIIN #
 HMIDIOUT #
 HMIDISTRM #
 HMIXEROBJ #
 HMIXER #
 HMMIO #
 HDROP #
 SC_HANDLE #
 SERVICE_STATUS_HANDLE #
 HIMC #
 HIMCC #
 NTSTATUS #
 EmfPlusRecordType #
m_mtd.AddList(s "")
 m_mi.Add("IUnknown" "")

 other handles will be catched by _struct()

 ________________________________

m_mi.Add("IUnknown" "interface IUnknown #QueryInterface(GUID*iid !*pObject) #AddRef() #Release() {00000000-0000-0000-C000-000000000046}")
m_mi.Add("IDispatch" "interface IDispatch :IUnknown[][9]#GetTypeInfoCount(*pctinfo)[][9]#GetTypeInfo(iTInfo lcid ITypeInfo*pTInfo)[][9]#GetIDsOfNames(GUID*riid word**rgszNames cNames lcid rgDispId)[][9]#Invoke(dispIdMember GUID*riid lcid @wFlags DISPPARAMS*pDispParams VARIANT*pVarResult EXCEPINFO*pExcepInfo *puArgErr)[][9]{00020400-0000-0000-C000-000000000046}")
m_mi.Add("IEnumVARIANT" "interface# IEnumVARIANT :IUnknown[][9]#Next(celt VARIANT*rgVar)[][9]Skip(celt)[][9]Reset()[][9]IEnumVARIANT'Clone()[][9]{00020404-0000-0000-C000-000000000046}") ;;FBC

 ________________________________

 g1
if(!empty(type_map)) m_mtd.AddList(type_map "")
