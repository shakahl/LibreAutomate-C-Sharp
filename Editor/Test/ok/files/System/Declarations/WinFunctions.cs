 #opt err 1
dll user32
	#CreateWindowEx dwExStyle $lpClassName $lpWindowName dwStyle X Y nWidth nHeight hWndParent hMenu hInstance !*lpParam
	#CreateWindowExW dwExStyle @*lpClassName @*lpWindowName dwStyle X Y nWidth nHeight hWndParent hMenu hInstance !*lpParam
	#DestroyWindow hWnd
	#EnableWindow hWnd fEnable
	#IsWindowEnabled hWnd
	#IsWindowVisible hWnd
	#IsWindow hWnd
	#IsChild hWndParent hWnd
	#GetWindowRect hWnd RECT*lpRect ;;see also: DpiGetWindowRect
	#SendMessage hWnd wMsg wParam lParam
	#SendMessageW hWnd wMsg wParam lParam
	#SendMessageTimeout hWnd msg wParam lParam fuFlags uTimeout *lpdwResult
	#SendMessageTimeoutW hWnd Msg wParam lParam fuFlags uTimeout *lpdwResult
	#SendNotifyMessage hWnd Msg wParam lParam
	#PostMessage hWnd wMsg wParam lParam
	PostQuitMessage nExitCode
	#DefWindowProc hWnd wMsg wParam lParam
	#DefWindowProcW hWnd Msg wParam lParam
	#CallWindowProc lpPrevWndFunc hWnd Msg wParam lParam
	#CallWindowProcW lpPrevWndFunc hWnd Msg wParam lParam
	#GetClassInfoEx hInstance $lpszClass WNDCLASSEX*lpwcx
	#GetClassInfoExW hInstance @*lpszClass WNDCLASSEXW*lpwcx
	@RegisterClassEx WNDCLASSEX*lpwcx
	@RegisterClassExW WNDCLASSEXW*lpwcx
	#UnregisterClassW @*lpClassName hInstance
	#MoveWindow hWnd x y nWidth nHeight bRepaint ;;see also: mov+
	#BringWindowToTop hWnd
	#IsZoomed hWnd
	#IsIconic hWnd
	#LoadCursor hInstance $lpCursorName
	#EndDialog hDlg nResult
	#SendDlgItemMessage hDlg nIDDlgItem wMsg wParam lParam
	#GetDlgItem hDlg nIDDlgItem
	#GetDlgItemInt hDlg nIDDlgItem lpTranslated bSigned
	#SetDlgItemInt hDlg nIDDlgItem wValue bSigned
	#CheckDlgButton hDlg nIDButton wCheck
	#CheckRadioButton hDlg nIDFirstButton nIDLastButton nIDCheckButton
	#IsDlgButtonChecked hDlg nIDButton
	#GetDlgCtrlID hWnd
	[GetDlgCtrlID]#GetWinId hWnd ;;Returns control id
	#SetDlgItemText hDlg nIDDlgItem $lpString
	#IsDialogMessage hDlg MSG*lpMsg
	#TranslateAccelerator hWnd hAccTable MSG*lpMsg
	#LoadAccelerators hInstance $lpTableName
	#CreateAcceleratorTable ACCEL*lpaccl cEntries
	#DestroyAcceleratorTable haccel
	#GetSystemMetrics nIndex
	#SystemParametersInfo uAction uParam lpvParam fuWinIni
	#SystemParametersInfoW uiAction uiParam !*pvParam fWinIni
	#GetSysColor nIndex
	#GetSysColorBrush nIndex
	#SetProp hWnd $lpString hData
	#GetProp hWnd $lpString
	#RemoveProp hWnd $lpString
	#AdjustWindowRectEx RECT*lpRect dsStyle bMenu dwEsStyle
	#SetWindowPos hWnd hWndInsertAfter x y cx cy wFlags ;;hWndInsertAfter and wFlags values are defined in WinConstants with HWND_ and SWP_ prefixes
	#GetWindowPlacement hWnd WINDOWPLACEMENT*lpwndpl
	#SetWindowPlacement hWnd WINDOWPLACEMENT*lpwndpl
	#ShowWindow hWnd nCmdShow ;;nCmdShow values are defined in WinConstants with SW_ prefix
	#EnumWindows lpEnumFunc lParam
	#EnumChildWindows hWndParent lpEnumFunc lParam
	#EnumThreadWindows dwThreadId lpfn lParam
	#GetWindow hWnd wFlag ;;wFlag: one of GW_... constants
	#GetTopWindow hWnd
	#GetParent hWnd ;;returns direct parent that can be control. Function GetAncestor(hwnd 2) returns top-level parent
	#SetParent hWndChild hWndNewParent
	#GetAncestor hWnd gaFlags ;;flags: 1 parent, 2 top-level parent, 3 top-level parent or owner
	#ScreenToClient hWnd POINT*lpPoint ;;see also: DpiScreenToClient
	#ClientToScreen hWnd POINT*lpPoint ;;see also: DpiClientToScreen
	#GetClientRect hWnd RECT*lpRect
	#MapWindowPoints hWndFrom hWndTo POINT*lppt cPoints ;;see also: DpiMapWindowPoints
	#GetWindowLong hWnd nIndex ;;nIndex values are defined in WinConstants with GWL_ prefix
	#SetWindowLong hWnd nIndex dwNewLong
	#SetWindowLongW hWnd nIndex dwNewLong
	#GetClassLong hWnd nIndex
	#RealGetWindowClassW hwnd @*ptszClassName cchClassNameMax
	#GetWindowTextLengthW hWnd
	#SetForegroundWindow hWnd
	#GetWindowThreadProcessId hWnd *lpdwProcessId
	#RedrawWindow hWnd RECT*lprcUpdate hrgnUpdate fuRedraw
	#GetCursorPos POINT*lpPoint ;;broken in DPI-scaled windows. Use xm instead.
	#SetCursorPos x y
	#ClipCursor RECT*lpRect
	#WindowFromPoint xPoint yPoint ;;see also: child(x y 0 1)
	#ChildWindowFromPoint hWnd xPoint yPoint ;;see also: child(x y hWnd 8|16)
	#ChildWindowFromPointEx hWnd POINT'pt uFlags ;;see also: child(x y hWnd 8|16)
	#SetCapture hWnd
	#ReleaseCapture
	#GetCapture
	#SetCursor hCursor
	#SetFocus hWnd
	#GetFocus
	#LoadImageW hinst @*lpszName uType cxDesired cyDesired fuLoad
	#DestroyIcon hIcon
	#DestroyCursor hCursor
	#DrawIconEx hDC xLeft yTop hIcon cxWidth cyWidth istepIfAniCur hbrFlickerFreeDraw diFlags
	#CopyIcon hIcon
	#CopyImage h type cx cy flags
	#GetCursorInfo CURSORINFO*pci
	#GetIconInfo hIcon ICONINFO*piconinfo
	#FillRect hDC RECT*lpRect hBrush
	#SetTimer hWnd nIDEvent uElapse lpTimerFunc ;;Use in dialog procedures and in toolbar hook functions. To set timer without hWnd, use tim or rep instead.
	#KillTimer hWnd nIDEvent
	mouse_event dwFlags dx dy cButtons dwExtraInfo
	keybd_event !bVk !bScan dwFlags dwExtraInfo
	SendInput nInputs INPUT*pInputs cbSize
	#BlockInput fBlockIt ;;fBlockIt: 1 block keyboard and mouse, 0 unblock; to unblock manually, press Ctrl+Alt+Delete
	#GetLastInputInfo LASTINPUTINFO*plii
	#GetKeyNameText lParam $lpString cchSize
	#CreatePopupMenu
	#TrackPopupMenuEx hMenu wFlags x y hWnd lptpm
	#DestroyMenu hMenu
	#AppendMenuW hMenu uFlags uIDNewItem @*lpNewItem
	#InsertMenuItemW hmenu item fByPosition MENUITEMINFOW*lpmi
	#EnableMenuItem hMenu wIDEnableItem wEnable
	#LoadMenu hInstance $lpString
	#SetMenu hWnd hMenu
	#GetMenu hWnd
	#GetMenuItemCount hMenu
	#GetMenuItemID hMenu nPos
	#GetSubMenu hMenu nPos
	#CheckMenuItem hMenu uIDCheckItem uCheck
	#DeleteMenu hMenu uPosition uFlags
	#CheckMenuRadioItem hmenu first last check flags
	#SetMenuDefaultItem hMenu uItem fByPos
	#CreateMenu
	#GetWindowContextHelpId hwnd
	#GetMenuInfo hmenu MENUINFO*lpcmi
	#GetMenuState hMenu uId uFlags
	#GetMenuItemInfoW hmenu item fByPosition MENUITEMINFOW*lpmii
	#GetMenuItemRect hWnd hMenu uItem RECT*lprcItem
	#MenuItemFromPoint hWnd hMenu POINT'ptScreen
	#GetDC hWnd
	#GetWindowDC hWnd
	#ReleaseDC hWnd hDC
	#RegisterWindowMessage $lpString
	#GetGUIThreadInfo idThread GUITHREADINFO*lpgui
	#GetMessage MSG*lpMsg hWnd wMsgFilterMin wMsgFilterMax
	#PeekMessage MSG*lpMsg hWnd wMsgFilterMin wMsgFilterMax wRemoveMsg
	#TranslateMessage MSG*lpMsg
	#DispatchMessage MSG*lpMsg
	#SetWindowsHookEx idHook lpfn hmod dwThreadId
	#UnhookWindowsHookEx hHook
	#CallNextHookEx hHook ncode wParam !*lParam
	#SetWinEventHook eventMin eventMax hmodWinEventProc lpfnWinEventProc idProcess idThread dwflags
	#UnhookWinEvent hWinEventHook
	#RegisterHotKey hWnd id fsModifiers vk
	#UnregisterHotKey hWnd id
	#CascadeWindows hWndParent wHow RECT*lpRect cKids *lpkids
	#TileWindows hWndParent wHow RECT*lpRect cKids *lpKids
	#BeginDeferWindowPos nNumWindows
	#DeferWindowPos hWinPosInfo hWnd hWndInsertAfter x y cx cy wFlags
	#EndDeferWindowPos hWinPosInfo
	#OpenClipboard hWnd
	#GetClipboardData wFormat
	#SetClipboardData wFormat hMem
	#EmptyClipboard
	#CloseClipboard
	#PtInRect RECT*lpRect ptX ptY
	#OffsetRect RECT*lpRect x y
	#ValidateRect hWnd RECT*lpRect
	#InvalidateRect hWnd RECT*lpRect bErase
	#FrameRect hDC RECT*lprc hbr
	#UpdateWindow hWnd
	#InflateRect RECT*lprc dx dy
	#DrawFocusRect hDC RECT*lprc
	#SetRect RECT*lprc xLeft yTop xRight yBottom
	#IsRectEmpty RECT*lprc
	#IntersectRect RECT*lprcDst RECT*lprcSrc1 RECT*lprcSrc2
	#EqualRect RECT*lprc1 RECT*lprc2
	#WindowFromDC hDC
	#DrawTextW hdc @*lpchText cchText RECT*lprc format
	#BeginPaint hWnd PAINTSTRUCT*lpPaint
	#EndPaint hWnd PAINTSTRUCT*lpPaint
	#MonitorFromPoint POINT'pt dwFlags ;;dwFlags: 0 default to 0, 1 default to primary, 2 default to nearest
	#MonitorFromRect RECT*lprc dwFlags ;;dwFlags: 0 default to 0, 1 default to primary, 2 default to nearest
	#MonitorFromWindow hwnd dwFlags ;;dwFlags: 0 default to 0, 1 default to primary, 2 default to nearest;  tip: if hwnd is 0 and dwFlags is 1, gets primary monitor
	#GetActiveWindow
	#SetLayeredWindowAttributes hwnd crKey !bAlpha dwFlags
	#GetLayeredWindowAttributes hwnd *pcrKey !*pbAlpha *pdwFlags
	#IsWindowUnicode hWnd
	#IsCharLowerW @ch
	#IsCharUpperW @ch
	@*CharLowerW @*lpsz ;;If lpsz is string, converts in place. If character, returns converted character. Use operator +.
	@*CharUpperW @*lpsz ;;If lpsz is string, converts in place. If character, returns converted character. Use operator +.
	#OemToChar $pSrc $pDst
	#AllowSetForegroundWindow dwProcessId ;;dwProcessId: -1 all processes
	#GetDesktopWindow
	#MapDialogRect hDlg RECT*lpRect
	#EnumDisplaySettings $lpszDeviceName iModeNum DEVMODE*lpDevMode
	#ChangeDisplaySettings DEVMODE*lpDevMode dwflags
	#GetComboBoxInfo hwndCombo COMBOBOXINFO*pcbi
	#FindWindowExW hWndParent hWndChildAfter @*lpszClass @*lpszWindow
	#SetScrollInfo hwnd nBar SCROLLINFO*lpsi redraw
	#GetScrollPos hWnd nBar
	#SetScrollPos hWnd nBar nPos bRedraw
	#ScrollDC hDC dx dy RECT*lprcScroll RECT*lprcClip hrgnUpdate RECT*lprcUpdate
	#IsHungAppWindow hwnd
	#GetShellWindow
	#GetLastActivePopup hWnd
	#PrintWindow hwnd hdcBlt nFlags

#opt hidedecl 1
dll- user32 [IsHungAppWindow]#IsHungWindow hwnd ;;Returns: 1 hung, 0 not hung or invalid handle
#opt hidedecl 0

dll kernel32
	#MultiByteToWideChar CodePage dwFlags $lpMultiByteStr cbMultiByte @*lpWideCharStr cchWideChar
	#WideCharToMultiByte CodePage dwFlags @*lpWideCharStr cchWideChar $lpMultiByteStr cbMultiByte $lpDefaultChar *lpUsedDefaultChar
	#MulDiv nNumber nNumerator nDenominator
	#GlobalAlloc wFlags dwBytes
	!*GlobalLock hMem
	#GlobalUnlock hMem
	#GlobalFree hMem
	#GlobalSize hMem
	#LocalFree hMem
	#GetCurrentThreadId
	#GetCurrentProcessId
	#GetCurrentDirectoryW nBufferLength @*lpBuffer
	#SetCurrentDirectoryW @*lpPathName
	#CreateFileW @*lpFileName dwDesiredAccess dwShareMode SECURITY_ATTRIBUTES*lpSecurityAttributes dwCreationDisposition dwFlagsAndAttributes hTemplateFile
	#WriteFile hFile !*lpBuffer nNumberOfBytesToWrite *lpNumberOfBytesWritten OVERLAPPED*lpOverlapped
	#ReadFile hFile !*lpBuffer nNumberOfBytesToRead *lpNumberOfBytesRead OVERLAPPED*lpOverlapped
	#SetFilePointerEx hFile LARGE_INTEGER'liDistanceToMove LARGE_INTEGER*lpNewFilePointer dwMoveMethod
	#GetFileSize hFile *lpFileSizeHigh
	#FindClose hFindFile
	#SetLocalTime SYSTEMTIME*lpSystemTime
	GetLocalTime SYSTEMTIME*lpSystemTime
	#SetSystemTime SYSTEMTIME*lpSystemTime
	GetSystemTime SYSTEMTIME*lpSystemTime
	#FileTimeToSystemTime FILETIME*lpFileTime SYSTEMTIME*lpSystemTime
	#SystemTimeToFileTime SYSTEMTIME*lpSystemTime FILETIME*lpFileTime
	#FileTimeToLocalFileTime FILETIME*lpFileTime FILETIME*lpLocalFileTime
	#LocalFileTimeToFileTime FILETIME*lpLocalFileTime FILETIME*lpFileTime
	#GetTickCount ;;returns number of milliseconds since the system was started
	#GetEnvironmentVariableW @*lpName @*lpBuffer nSize
	#SetEnvironmentVariableW @*lpName @*lpValue
	#GetLastError
	SetLastError dwErrCode
	#OpenProcess dwDesiredAccess bInheritHandle dwProcessId
	#TerminateProcess hProcess uExitCode
	#CloseHandle hObject
	#QueryPerformanceCounter long*lpPerformanceCount
	#QueryPerformanceFrequency long*lpFrequency
	#CreateEvent SECURITY_ATTRIBUTES*lpEventAttributes bManualReset bInitialState $lpName
	#SetEvent hEvent
	#PulseEvent hEvent
	#ResetEvent hEvent
	#WaitForSingleObject hHandle dwMilliseconds
	#GetVersionEx OSVERSIONINFO*lpVersionInformation
	#GetFileAttributesExW @*lpFileName fInfoLevelId !*lpFileInformation
	#GetFileAttributesW @*lpFileName
	#SetFileAttributesW @*lpFileName dwFileAttributes
	#SetFileTime hFile FILETIME*lpCreationTime FILETIME*lpLastAccessTime FILETIME*lpLastWriteTime
	#GetDiskFreeSpaceEx $lpDirectoryName long*lpFreeBytesAvailable long*lpTotalNumberOfBytes long*lpTotalNumberOfFreeBytes
	#IsBadReadPtr !*lp ucb
	#IsBadWritePtr !*lp ucb
	#IsBadCodePtr lpfn
	#CreateToolhelp32Snapshot dwFlags th32ProcessID
	#Process32FirstW hSnapshot PROCESSENTRY32W*lppe
	#Process32NextW hSnapshot PROCESSENTRY32W*lppe
	#Thread32First hSnapshot THREADENTRY32*lpte
	#Thread32Next hSnapshot THREADENTRY32*lpte
	#GetProcessTimes hProcess FILETIME*lpCreationTime FILETIME*lpExitTime FILETIME*lpKernelTime FILETIME*lpUserTime
	#DeleteFileW @*lpFileName
	#CopyFileExW @*lpExistingFileName @*lpNewFileName lpProgressRoutine !*lpData *pbCancel dwCopyFlags
	#MoveFileExW @*lpExistingFileName @*lpNewFileName dwFlags
	#CreateDirectoryExW @*lpTemplateDirectory @*lpNewDirectory SECURITY_ATTRIBUTES*lpSecurityAttributes
	#RemoveDirectoryW @*lpPathName
	#GetCurrentProcess
	#CreatePipe *hReadPipe *hWritePipe SECURITY_ATTRIBUTES*lpPipeAttributes nSize
	#DuplicateHandle hSourceProcessHandle hSourceHandle hTargetProcessHandle *lpTargetHandle dwDesiredAccess bInheritHandle dwOptions
	#CreateProcessW @*lpApplicationName @*lpCommandLine SECURITY_ATTRIBUTES*lpProcessAttributes SECURITY_ATTRIBUTES*lpThreadAttributes bInheritHandles dwCreationFlags !*lpEnvironment @*lpCurrentDirectory STARTUPINFOW*lpStartupInfo PROCESS_INFORMATION*lpProcessInformation
	#GetExitCodeProcess hProcess *lpExitCode
	#LoadLibraryW @*lpLibFileName
	#GetModuleHandle $lpModuleName
	#GetModuleFileNameW hModule @*lpFilename nSize
	#PeekNamedPipe hNamedPipe !*lpBuffer nBufferSize *lpBytesRead *lpTotalBytesAvail *lpBytesLeftThisMessage
	!*VirtualAllocEx hProcess !*lpAddress dwSize flAllocationType flProtect
	#VirtualFreeEx hProcess !*lpAddress dwSize dwFreeType
	#WriteProcessMemory hProcess !*lpBaseAddress !*lpBuffer nSize *lpNumberOfBytesWritten
	#ReadProcessMemory hProcess !*lpBaseAddress !*lpBuffer nSize *lpNumberOfBytesRead
	GetSystemTimeAsFileTime FILETIME*lpSystemTimeAsFileTime
	$lstrcpy $sTo $sFrom
	$lstrcpyn $sTo $sFrom iMaxLength
	@*lstrcpyW @*sTo @*sFrom
	@*lstrcpynW @*sTo @*sFrom iMaxLength
	#lstrlenW @*lpString
	@*GetEnvironmentStringsW
	#FreeEnvironmentStringsW @*lpszEnvironmentBlock
	#CreateFileMappingW hFile SECURITY_ATTRIBUTES*lpFileMappingAttributes flProtect dwMaximumSizeHigh dwMaximumSizeLow @*lpName
	#OpenFileMappingW dwDesiredAccess bInheritHandle @*lpName
	!*MapViewOfFile hFileMappingObject dwDesiredAccess dwFileOffsetHigh dwFileOffsetLow dwNumberOfBytesToMap
	#UnmapViewOfFile !*lpBaseAddress
	#GetStdHandle nStdHandle
	#AllocConsole
	#GetConsoleMode hConsoleHandle *lpMode
	@GlobalAddAtom $lpString
	#CreateActCtxW ACTCTXW*pActCtx
	ReleaseActCtx hActCtx
	#ActivateActCtx hActCtx *lpCookie
	#DeactivateActCtx dwFlags ulCookie
	#SetHandleInformation hObject dwMask dwFlags

dll- kernel32
	#GetApplicationUserModelId hProcess *applicationUserModelIdLength @*applicationUserModelId

dll gdi32
	#CreateCompatibleDC hDC
	#GetStockObject nIndex
	#GetDeviceCaps hDC nIndex
	#CreateFontIndirectW LOGFONTW*lplf
	#SelectObject hDC hObject
	#DeleteDC hDC
	#CreateSolidBrush crColor
	#CreatePatternBrush hbm
	#CreatePen iStyle cWidth color
	#DeleteObject hObject
	#InvertRgn hDC hRgn
	#CreateCompatibleBitmap hDC nWidth nHeight
	#BitBlt hDestDC x y nWidth nHeight hSrcDC xSrc ySrc dwRop
	#SetTextColor hDC crColor
	#GetTextColor hdc
	#SetBkMode hDC nBkMode
	#SetBkColor hdc crColor
	#StretchBlt hdcDest nXOriginDest nYOriginDest nWidthDest nHeightDest hdcSrc nXOriginSrc nYOriginSrc nWidthSrc nHeightSrc dwRop
	#GetPixel hdc nXPos nYPos
	#GetObjectW hgdiobj cbBuffer !*lpvObject
	#GetViewportOrgEx hdc POINT*lppoint
	#SetViewportOrgEx hdc x y POINT*lppt
	#SetBrushOrgEx hdc x y POINT*lppt
	#GdiGradientFill hdc TRIVERTEX*pVertex nVertex !*pMesh nCount ulMode
	#CreateRectRgnIndirect RECT*lprect
	#FrameRgn hdc hrgn hbr w h
	#SetRectRgn hrgn left top right bottom
	#SaveDC hdc
	#RestoreDC hdc nSavedDC
	#SetDCPenColor hdc color
	#Rectangle hdc left top right bottom
	#TextOutW hdc x y @*lpString c
	#GetTextMetricsW hdc TEXTMETRICW*lptm
	#OffsetViewportOrgEx hdc x y POINT*lppt

dll shell32
	#Shell_NotifyIconW dwMessage NOTIFYICONDATAW*lpData
	#DragQueryFileW hDrop iFile @*lpszFile cch 
	ITEMIDLIST*SHBrowseForFolderW BROWSEINFOW*lpbi
	[62]#PickIconDlg hwnd @*pszIconPath cbIconPath *piIconIndex
	[680]#IsUserAnAdmin
	SHChangeNotify wEventId uFlags !*dwItem1 !*dwItem2
	@**CommandLineToArgvW @*lpCmdLine *pNumArgs

dll shlwapi
	#SHAutoComplete hwndEdit dwFlags
	#SHCreateStreamOnFileW @*pszFile grfMode IStream*ppstm
	#SHCopyKeyW hkeySrc @*wszSrcSubKey hkeyDest fReserved
	#PathCompactPathExW @*pszOut @*pszSrc cchMax dwFlags
	#PathIsRelative $pszPath
	#PathParseIconLocation $pszIconFile
	#ColorAdjustLuma clrRGB n fScale
	[156]#StrCmpCW @*pszStr1 @*pszStr2

dll comctl32
	#ImageList_Create cx cy flags cInitial cGrow
	#ImageList_Destroy himl
	#ImageList_ReplaceIcon himl i hicon
	#ImageList_SetOverlayImage himl iImage iOverlay
	#ImageList_Draw himl i hdcDst x y fStyle
	[410]#SetWindowSubclass hWnd pfnSubclass uIdSubclass dwRefData   ;;pfnSubclass: function# hWnd uMsg wParam lParam uIdSubclass dwRefData
	[411]#GetWindowSubclass hWnd pfnSubclass uIdSubclass *pdwRefData
	[412]#RemoveWindowSubclass hWnd pfnSubclass uIdSubclass
	[413]#DefSubclassProc hWnd uMsg wParam lParam

dll advapi32
	#RegCreateKeyExW hKey @*lpSubKey Reserved @*lpClass dwOptions samDesired SECURITY_ATTRIBUTES*lpSecurityAttributes *phkResult *lpdwDisposition
	#RegOpenKeyExW hKey @*lpSubKey ulOptions samDesired *phkResult
	#RegCloseKey hKey
	#RegEnumKeyExW hKey dwIndex @*lpName *lpcbName *lpReserved @*lpClass *lpcbClass FILETIME*lpftLastWriteTime
	#RegEnumValueW hKey dwIndex @*lpValueName *lpcbValueName *lpReserved *lpType !*lpData *lpcbData
	#CreateProcessWithLogonW @*lpUsername @*lpDomain @*lpPassword dwLogonFlags @*lpApplicationName @*lpCommandLine dwCreationFlags !*lpEnvironment @*lpCurrentDirectory STARTUPINFOW*lpStartupInfo PROCESS_INFORMATION*lpProcessInformation

dll ole32
	!*CoTaskMemAlloc cb
	#CoTaskMemFree !*pv
	#CoCreateGuid GUID*pguid
	#StringFromIID GUID*rclsid @**lplpsz
	#CreateStreamOnHGlobal hGlobal fDeleteOnRelease IStream*ppstm
	#GetHGlobalFromStream IStream'pstm *phglobal
	#CoMarshalInterThreadInterfaceInStream GUID*riid IUnknown'pUnk IStream*ppStm
	#CoGetInterfaceAndReleaseStream IStream'pStm GUID*iid !**ppv

dll oleaut32
	#VariantTimeToSystemTime ^vtime SYSTEMTIME*lpSystemTime
	#SystemTimeToVariantTime SYSTEMTIME*lpSystemTime ^*pvtime
	#VarBstrCmp BSTR'bstrLeft BSTR'bstrRight lcid dwFlags

dll- oleacc
	#AccessibleObjectFromEvent hwnd dwId dwChildId !*ppacc VARIANT*pvarChild
	#AccessibleObjectFromWindow hwnd dwId GUID*riid !**ppvObject
	#GetRoleTextW lRole @*lpszRole cchRoleMax
	#GetStateTextW lStateBit @*lpszState cchState
	#GetProcessHandleFromHwnd hwnd

dll- comdlg32
	#ChooseFontW CHOOSEFONTW*lpcf
	#ChooseColor CHOOSECOLOR*pChoosecolor
	#GetOpenFileNameW OPENFILENAMEW*lpofn
	#GetSaveFileNameW OPENFILENAMEW*lpofn
	#CommDlgExtendedError

#opt hidedecl 1
dll- wininet
	#InternetGetConnectedState *lpdwFlags dwReserved
	#InternetGoOnline $lpszURL hwndParent dwFlags
	[InternetSetOptionA]#InternetSetOption hInternet dwOption !*lpBuffer dwBufferLength
	#InternetCheckConnectionW @*lpszUrl dwFlags dwReserved
	#InternetDialW hwndParent @*lpszConnectoid dwFlags *lpdwConnection dwReserved
	[InternetOpenA]#InternetOpen $lpszAgent dwAccessType $lpszProxy $lpszProxyBypass dwFlags
	#InternetCloseHandle hInternet
	#InternetConnectW hInternet @*lpszServerName @nServerPort @*lpszUserName @*lpszPassword dwService dwFlags dwContext
	#InternetGetLastResponseInfoW *lpdwError @*lpszBuffer *lpdwBufferLength
	[InternetFindNextFileA]#InternetFindNextFile hFind !*lpvFindData
	#InternetWriteFile hFile !*lpBuffer dwNumberOfBytesToWrite *lpdwNumberOfBytesWritten
	#InternetQueryDataAvailable hFile *lpdwNumberOfBytesAvailable dwFlags dwContext
	#InternetReadFile hFile !*lpBuffer dwNumberOfBytesToRead *lpdwNumberOfBytesRead
	[InternetOpenUrlA]#InternetOpenUrl hInternet $lpszUrl $lpszHeaders dwHeadersLength dwFlags dwContext
	#InternetHangUp dwConnection dwReserved
	[InternetCrackUrlA]#InternetCrackUrl $lpszUrl dwUrlLength dwFlags URL_COMPONENTS*lpUrlComponents
	#FtpSetCurrentDirectoryW hConnect @*lpszDirectory
	[FtpFindFirstFileA]#FtpFindFirstFile hConnect $lpszSearchFile WIN32_FIND_DATA*lpFindFileData dwFlags dwContext
	#FtpGetFileW hConnect @*lpszRemoteFile @*lpszNewFile fFailIfExists dwFlagsAndAttributes dwFlags dwContext
	#FtpDeleteFileW hConnect @*lpszFileName
	#FtpPutFileW hConnect @*lpszLocalFile @*lpszNewRemoteFile dwFlags dwContext
	#FtpGetCurrentDirectoryW hConnect @*lpszCurrentDirectory *lpdwCurrentDirectory
	#FtpCreateDirectoryW hConnect @*lpszDirectory
	#FtpRemoveDirectoryW hConnect @*lpszDirectory
	#FtpRenameFileW hConnect @*lpszExisting @*lpszNew
	#FtpCommandW hConnect fExpectResponse dwFlags @*lpszCommand dwContext *phFtpCommand
	#FtpGetFileSize hFile *lpdwFileSizeHigh
	#FtpOpenFileW hConnect @*lpszFileName dwAccess dwFlags dwContext
	[HttpQueryInfoA]#HttpQueryInfo hRequest dwInfoLevel !*lpBuffer *lpdwBufferLength *lpdwIndex
	[HttpOpenRequestA]#HttpOpenRequest hConnect $lpszVerb $lpszObjectName $lpszVersion $lpszReferrer $*lplpszAcceptTypes dwFlags dwContext
	[HttpSendRequestA]#HttpSendRequest hRequest $lpszHeaders dwHeadersLength !*lpOptional dwOptionalLength
	[HttpSendRequestExA]#HttpSendRequestEx hRequest INTERNET_BUFFERS*lpBuffersIn INTERNET_BUFFERS*lpBuffersOut dwFlags dwContext
	[HttpEndRequestA]#HttpEndRequest hRequest INTERNET_BUFFERS*lpBuffersOut dwFlags dwContext
#opt hidedecl 0

dll- urlmon #FindMimeFromData !*pBC @*pwzUrl !*pBuffer cbSize @*pwzMimeProposed dwMimeFlags @**ppwzMimeOut dwReserved

dll- uxtheme
	#SetWindowTheme hwnd @*pszSubAppName @*pszSubIdList
	#OpenThemeData hwnd @*pszClassList
	#CloseThemeData hTheme

dll- winmm
	#timeGetTime
	#waveOutSetVolume hwo dwVolume

dll- wtsapi32 #WTSTerminateProcess hServer ProcessId ExitCode

dll- ws2_32 hostent*gethostbyname $name

dll- "winspool.drv" #SetDefaultPrinterW @*pszPrinter

dll- "dwmapi"
	#DwmIsCompositionEnabled *pfEnabled
	#DwmGetWindowAttribute hwnd dwAttribute !*pvAttribute cbAttribute
