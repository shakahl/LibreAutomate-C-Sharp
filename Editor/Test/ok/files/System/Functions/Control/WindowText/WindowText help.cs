 Class WindowText captures window text and stores in an array of text items.

 Where to use?
 Find/click/waitfor text in a window where other functions (win, child, Acc, etc) can't do it.
 Get window text formatted as single string or as array of text items.

 What is a text item?
 A window usually draws each logically separate text string separately. We capture the string as a text item.
 For example, if it is a list, each list item is drawn separately. Multiline text also often is drawn each line separately.
 To see all text items in a window/control from mouse, click Test in the 'Window Text' dialog or run function WindowText_Test.

 To store properties of captured text items, a WindowText variable has an internal array of type WTI:
 type WTI hwnd $txt txtLen RECT'rt RECT'rv @fontHeight !flags !api apiFlags color
   hwnd - handle of window containing the text. It may be the target window or one of its children/descendants.
   txt - text.
   txtLen - text length.
   rt - text rectangle. Coordinates are relative to client area of hwnd; if need in screen or other window, use DpiMapWindowPoints to convert; see examples in WindowText.Find and WT_ResultsVisual.
   rv - part of rt that is visible in window (and in r, if used). Can be used to click. Empty if invisible (flags&WTI_INVISIBLE).
   fontHeight - font height, including space above and below. If single line, it usually = rt.bottom-rt.top.
   flags:
     WTI_INVISIBLE (1) - is invisible in window. Eg scrolled somewhere or covered by other windows.
     WTI_MULTILINE (2) - is multiline text.
   api - what API function was used to draw text. Currently undocumented.
   apiFlags - flags passed to the API. Currently undocumented. For console windows, apiFlags is font width.
   color - text color in 0xBBGGRR format.
   bkColor - background color. -1 if cannot get or WT_GETBKCOLOR flag not used with Init.

 Does not use OCR, therefore is fast, accurate, and works in backround windows too. However the window cannot be hidden or minimized.
 Works not with all windows. Does not work with web browsers, browser controls, Java, WPF, Windows store apps; these may be supported in the future.
 Like most UI automation functions, does not work on Windows Vista and later if used in process running as User whereas the target process has higher UAC integrity level (Administrator or uiAccess).
 Does not capture top-level window title and standard menu bar.
 Uses 2 dll files: qmtc32.dll and qmtc64.dll (for 64-bit windows). They are in QM folder. If used in exe on other computers, they must be in exe folder.

 With Debug version of text capturing dll, you can use this to enable debug info:
 SetProp(_hwndqm "qmtc_debug_output" 1) ;;set to 0 to disable again


 EXAMPLES
 See in help of Capture(), Find() and other functions.
 See also WindowText_Test.
