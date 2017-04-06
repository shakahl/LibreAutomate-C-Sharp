function hwnd [RECT&r] [flags]

 Sets target window, rectangle and flags for text capturing.

 hwnd - handle of window to capture. Can be child or top-level window.
 r - a RECT variable that specifies a rectangle in window (hwnd).
    If used, gets only text items that are in the rectangle. For items that partially intersect with the rectangle, gets whole text and WTI.rt, but clips WTI.rv.
    If omitted or 0, gets all.
    Coordinates must be relative to hwnd client area.
 flags - text capturing options:
   WT_SPLITMULTILINE (1) - if text is multiline, get separate WTIs for each line. Without this, gets single WTI.
   WT_JOIN (2) - join adjacent WTIs into single WTI when drawn in a line from left to right, with no gap.
   WT_JOINMORE (4) - join adjacent WTIs into single WTI when drawn in a line from left to right, possibly with gap.
   WT_NOCHILDREN (8) - don't get text from child/descendant windows of hwnd.
   WT_VISIBLE (16) - don't get text items that are not really visible in window, eg covered by other child windows or scrolled somewhere.
     Note: with or without this flag, never gets text from not really visible windows (completely covered etc) and text that is not in r.
   WT_REDRAW (32) - use a slightly different way to get text. In some cases it may work better, but it causes flickering and cannot get all text from background windows.
   WT_SORT (64) - sort to match normal reading direction. Sorts in each window separately, unless WT_SINGLE_COORD_SYSTEM.
   WT_SINGLE_COORD_SYSTEM (128) - get all WTI.rt/WTI.rv coordinates relative to client area of hwnd. Without this flag, coordinates are in client area of the window that draws text; it may be a child/descendant of hwnd, unless WT_NOCHILDREN.
   WT_NOCLIPTEXT (0x400) - don't apply initial text clipping.
   WT_GETBKCOLOR (0x800) - get background color. Slower.

 REMARKS
 This function does not capture text. Just sets capturing parameters for Capture(), which also is called by Find(), Wait() and CaptureToString().
 You can call Init() multiple times, for example to change target window, rectangle or flags.


if(!hwnd) end ERR_WINDOW

if(hwnd!=m_hwnd) m_dpiScaled=DpiIsWindowScaled(hwnd)
m_hwnd=hwnd
m_flags=flags
if(&r) m_r=r; m_rp=&m_r; if(m_dpiScaled) DpiScale +m_rp -2
else m_rp=0
m_captured=0 ;;if was captured, let next Find() recapture

err+ end _error

 undocumented, can be used with Capture():
 WT_WAIT_BEGIN=0x100, //capture current text as usual, but don't stop capturing new text until Capture called with WT_WAIT_END.
 WT_WAIT=0x300, //get new text captured since Capture called with WT_WAIT_BEGIN or WT_WAIT. If previously not called with WT_WAIT_BEGIN, starts capturing new text but does not capture existing text.
 WT_WAIT_END=0x200, //get new text captured since Capture called with WT_WAIT_BEGIN or WT_WAIT, and end capturing.
 //WT_WAITx flags cannot be used together.
