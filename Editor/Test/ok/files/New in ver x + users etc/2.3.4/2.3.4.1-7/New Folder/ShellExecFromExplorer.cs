 /Macro1906
function! $file_ [$args]

Q &q
IShellWindows psw;
if(CoCreateInstance(CLSID_ShellWindows, 0, CLSCTX_LOCAL_SERVER, IID_IShellWindows &psw)) ret
int hwnd;
IDispatch pdisp;
VARIANT vEmpty
if(psw.FindWindowSW(&vEmpty, &vEmpty, SWC_DESKTOP, &hwnd, SWFO_NEEDDISPATCH, &pdisp)) ret
IShellBrowser psb
if(IUnknown_QueryService(pdisp, SID_STopLevelBrowser, IID_IShellBrowser &psb)) ret
IShellView psv;
if(psb.QueryActiveShellView(&psv)) ret
IDispatch pdispBackground;
if(psv.GetItemObject(SVGIO_BACKGROUND, IID_IDispatch &pdispBackground)) ret
IShellFolderViewDual psfvd=+pdispBackground
IDispatch pdisp2;
if(psfvd.get_Application(&pdisp2)) ret
IShellDispatch2 psd=+pdisp2
 out psd

str s.expandpath(file_)
VARIANT vcd=GetCurDir

 AllowActivateWindows

Q &qq
psd.ShellExecuteA(s @ vcd @ 1)
Q &qqq
outq
