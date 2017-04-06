#opt hidedecl 1
type WNDCLASSEX cbSize style lpfnWndProc cbClsExtra cbWndExtra hInstance hIcon hCursor hbrBackground $lpszMenuName $lpszClassName hIconSm
type WNDCLASSEXW cbSize style lpfnWndProc cbClsExtra cbWndExtra hInstance hIcon hCursor hbrBackground @*lpszMenuName @*lpszClassName hIconSm
type TEXTRANGE CHARRANGE'chrg $lpstrText
type NOTIFYICONDATAW cbSize hWnd uID uFlags uCallbackMessage hIcon @szTip[128] dwState dwStateMask @szInfo[256] {uTimeout []uVersion} @szInfoTitle[64] dwInfoFlags
type MSG hwnd message wParam lParam time POINT'pt
type CREATESTRUCTW !*lpCreateParams hInstance hMenu hwndParent cy cx y x style @*lpszName @*lpszClass dwExStyle
type DLGTEMPLATEEX @dlgVer @signature helpID exStyle style @cDlgItems @x @y @cx @cy
type DLGITEMTEMPLATEEX helpId exStyle style @x @y @cx @cy @id
type SECURITY_ATTRIBUTES nLength !*lpSecurityDescriptor bInheritHandle
type LARGE_INTEGER {lowpart highpart} []%quadpart
type __ULARGE_INTEGER2 LowPart HighPart
type ULARGE_INTEGER {LowPart HighPart} []__ULARGE_INTEGER2'u []%QuadPart
type SHITEMID @cb !abID[1]
type ITEMIDLIST SHITEMID'mkid
type SIZE cx cy
type POINTL x y
type WINDOWPLACEMENT Length flags showCmd POINT'ptMinPosition POINT'ptMaxPosition RECT'rcNormalPosition
type MENUITEMINFOW cbSize fMask fType fState wID hSubMenu hbmpChecked hbmpUnchecked dwItemData @*dwTypeData cch hbmpItem
type MENUINFO cbSize fMask dwStyle cyMax hbrBack dwContextHelpID dwMenuData
type MOUSEINPUT dx dy mouseData dwFlags time dwExtraInfo
type KEYBDINPUT @wVk @wScan dwFlags time dwExtraInfo
type HARDWAREINPUT uMsg @wParamL @wParamH
type INPUT type MOUSEINPUT'mi []KEYBDINPUT'ki []HARDWAREINPUT'hi
type GUITHREADINFO cbSize flags hwndActive hwndFocus hwndCapture hwndMenuOwner hwndMoveSize hwndCaret RECT'rcCaret
type OSVERSIONINFO dwOSVersionInfoSize dwMajorVersion dwMinorVersion dwBuildNumber dwPlatformId !szCSDVersion[128]
type BROWSEINFOW hwndOwner ITEMIDLIST*pidlRoot @*pszDisplayName @*lpszTitle ulFlags lpfn lParam iImage
type MEASUREITEMSTRUCT CtlType CtlID itemID itemWidth itemHeight itemData
type DRAWITEMSTRUCT CtlType CtlID itemID itemAction itemState hWndItem hDC RECT'rcItem itemData
type ACCEL !fVirt @key @cmd
type OVERLAPPED Internal InternalHigh offset OffsetHigh hEvent
type PROCESSENTRY32W dwSize cntUsage th32ProcessID th32DefaultHeapID th32ModuleID cntThreads th32ParentProcessID pcPriClassBase dwFlags @szExeFile[260]
type THREADENTRY32 dwSize cntUsage th32ThreadID th32OwnerProcessID tpBasePri tpDeltaPri dwFlags
type LOGFONTW lfHeight lfWidth lfEscapement lfOrientation lfWeight !lfItalic !lfUnderline !lfStrikeOut !lfCharSet !lfOutPrecision !lfClipPrecision !lfQuality !lfPitchAndFamily @lfFaceName[32]
type CHOOSEFONTW lStructSize hwndOwner hDC LOGFONTW*lpLogFont iPointSize Flags rgbColors lCustData lpfnHook @*lpTemplateName hInstance @*lpszStyle @nFontType @___MISSING_ALIGNMENT__ nSizeMin nSizeMax
type CHOOSECOLOR lStructSize hWndOwner hInstance rgbResult *lpCustColors flags lCustData lpfnHook $lpTemplateName
type OPENFILENAMEW lStructSize hwndOwner hInstance @*lpstrFilter @*lpstrCustomFilter nMaxCustFilter nFilterIndex @*lpstrFile nMaxFile @*lpstrFileTitle nMaxFileTitle @*lpstrInitialDir @*lpstrTitle Flags @nFileOffset @nFileExtension @*lpstrDefExt lCustData lpfnHook @*lpTemplateName !*pvReserved dwReserved FlagsEx
type PAINTSTRUCT hDC fErase RECT'rcPaint fRestore fIncUpdate !rgbReserved[32]
type DVTARGETDEVICE tdSize @tdDriverNameOffset @tdDeviceNameOffset @tdPortNameOffset @tdExtDevmodeOffset !tdData[1]
type FORMATETC @cfFormat DVTARGETDEVICE*ptd dwAspect lindex tymed
type COPYDATASTRUCT dwData cbData !*lpData
type WIN32_FIND_DATA dwFileAttributes FILETIME'ftCreationTime FILETIME'ftLastAccessTime FILETIME'ftLastWriteTime nFileSizeHigh nFileSizeLow dwReserved0 dwReserved1 !cFileName[260] !cAlternate[14]
type WIN32_FILE_ATTRIBUTE_DATA dwFileAttributes FILETIME'ftCreationTime FILETIME'ftLastAccessTime FILETIME'ftLastWriteTime nFileSizeHigh nFileSizeLow
type INTERNET_CONNECTED_INFO dwConnectedState dwFlags
type INTERNET_BUFFERSA dwStructSize INTERNET_BUFFERSA*Next $lpcszHeader dwHeadersLength dwHeadersTotal !*lpvBuffer dwBufferLength dwBufferTotal dwOffsetLow dwOffsetHigh
type INTERNET_BUFFERS dwStructSize INTERNET_BUFFERSA*Next $lpcszHeader dwHeadersLength dwHeadersTotal !*lpvBuffer dwBufferLength dwBufferTotal dwOffsetLow dwOffsetHigh
type URL_COMPONENTS dwStructSize $lpszScheme dwSchemeLength nScheme $lpszHostName dwHostNameLength @nPort $lpszUserName dwUserNameLength $lpszPassword dwPasswordLength $lpszUrlPath dwUrlPathLength $lpszExtraInfo dwExtraInfoLength
type IMAGEINFO hbmImage hbmMask Unused1 Unused2 RECT'rcImage
type BITMAP bmType bmWidth bmHeight bmWidthBytes @bmPlanes @bmBitsPixel !*bmBits
type PROCESS_INFORMATION hProcess hThread dwProcessId dwThreadId
type STARTUPINFOW cb @*lpReserved @*lpDesktop @*lpTitle dwX dwY dwXSize dwYSize dwXCountChars dwYCountChars dwFillAttribute dwFlags @wShowWindow @cbReserved2 !*lpReserved2 hStdInput hStdOutput hStdError
type LASTINPUTINFO cbSize dwTime
type CURSORINFO cbSize flags hCursor POINT'ptScreenPos
type ICONINFO fIcon xHotspot yHotspot hbmMask hbmColor
type KBDLLHOOKSTRUCT vkCode scanCode flags time dwExtraInfo
type MSLLHOOKSTRUCT POINT'pt mouseData flags time dwExtraInfo
type DEVMODE !dmDeviceName[32] @dmSpecVersion @dmDriverVersion @dmSize @dmDriverExtra dmFields {{@dmOrientation @dmPaperSize @dmPaperLength @dmPaperWidth @dmScale @dmCopies @dmDefaultSource @dmPrintQuality} []{POINTL'dmPosition dmDisplayOrientation dmDisplayFixedOutput}} @dmColor @dmDuplex @dmYResolution @dmTTOption @dmCollate !dmFormName[32] @dmLogPixels dmBitsPerPel dmPelsWidth dmPelsHeight {dmDisplayFlags []dmNup} dmDisplayFrequency dmICMMethod dmICMIntent dmMediaType dmDitherType dmReserved1 dmReserved2 dmPanningWidth dmPanningHeight
type WSADATA @wVersion @wHighVersion !szDescription[257] !szSystemStatus[129] @iMaxSockets @iMaxUdpDg $lpVendorInfo
type hostent $h_name $*h_aliases @h_addrtype @h_length $*h_addr_list
type NONCLIENTMETRICSW cbSize iBorderWidth iScrollWidth iScrollHeight iCaptionWidth iCaptionHeight LOGFONTW'lfCaptionFont iSmCaptionWidth iSmCaptionHeight LOGFONTW'lfSmCaptionFont iMenuWidth iMenuHeight LOGFONTW'lfMenuFont LOGFONTW'lfStatusFont LOGFONTW'lfMessageFont
type TRIVERTEX x y @Red @Green @Blue @Alpha
type GRADIENT_RECT UpperLeft LowerRight
type ACTCTXW cbSize dwFlags @*lpSource @wProcessorArchitecture @wLangId @*lpAssemblyDirectory @*lpResourceName @*lpApplicationName hModule
type TEXTMETRICW tmHeight tmAscent tmDescent tmInternalLeading tmExternalLeading tmAveCharWidth tmMaxCharWidth tmWeight tmOverhang tmDigitizedAspectX tmDigitizedAspectY @tmFirstChar @tmLastChar @tmDefaultChar @tmBreakChar !tmItalic !tmUnderlined !tmStruckOut !tmPitchAndFamily !tmCharSet [pack4]
type STATSTG @*pwcsName type ULARGE_INTEGER'cbSize FILETIME'mtime FILETIME'ctime FILETIME'atime grfMode grfLocksSupported GUID'clsid grfStateBits reserved
type SCROLLINFO cbSize fMask nMin nMax nPage nPos nTrackPos

type NMHDR hwndFrom idFrom code
type NMCUSTOMDRAW NMHDR'hdr dwDrawStage hdc RECT'rc dwItemSpec uItemState lItemlParam
type LVITEM mask iItem iSubItem state stateMask $pszText cchTextMax iImage lParam iIndent iGroupId cColumns *puColumns
type LVITEMW mask iItem iSubItem state stateMask @*pszText cchTextMax iImage lParam iIndent iGroupId cColumns *puColumns
type LVCOLUMNW mask fmt cx @*pszText cchTextMax iSubItem iImage iOrder
type NMLISTVIEW NMHDR'hdr iItem iSubItem uNewState uOldState uChanged POINT'ptAction lParam
type NMLVKEYDOWN NMHDR'hdr @wVKey flags
type NMLVDISPINFO NMHDR'hdr LVITEM'item
type NMLVDISPINFOW NMHDR'hdr LVITEMW'item
type NMITEMACTIVATE NMHDR'hdr iItem iSubItem uNewState uOldState uChanged POINT'ptAction lParam uKeyFlags
type TVITEMW mask hItem state stateMask @*pszText cchTextMax iImage iSelectedImage cChildren lParam
type TVITEMEXW :TVITEMW iIntegral uStateEx hwnd iExpandedImage
type TVINSERTSTRUCTW hParent hInsertAfter {TVITEMEXW'itemex []TVITEMW'item}
type TVHITTESTINFO POINT'pt flags hItem
type NMTREEVIEWW NMHDR'hdr action TVITEMW'itemOld TVITEMW'itemNew POINT'ptDrag
type NMTVDISPINFOW NMHDR'hdr TVITEMW'item
type NMTVCUSTOMDRAW NMCUSTOMDRAW'nmcd clrText clrTextBk iLevel
type TBBUTTON iBitmap idCommand !fsState !fsStyle !bReserved[2] dwData iString
type NMTOOLBAR NMHDR'hdr iItem TBBUTTON'tbButton cchText $pszText RECT'rcButton
type TOOLINFOW cbSize uFlags hwnd uId RECT'rect hinst @*lpszText lParam ;;!*lpReserved
type TCITEMW mask dwState dwStateMask @*pszText cchTextMax iImage lParam
type COMBOBOXINFO cbSize RECT'rcItem RECT'rcButton stateButton hwndCombo hwndItem hwndList
type LITEM mask iLink state stateMask @szID[48] @szUrl[2084]
type NMLINK NMHDR'hdr LITEM'item
#opt hidedecl 0
