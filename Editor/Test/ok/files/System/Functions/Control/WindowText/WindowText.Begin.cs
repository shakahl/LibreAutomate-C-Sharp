
 Begins text capturing. Optional.

 hwnd - handle of window to capture. Can be child or top-level window.

 REMARKS
 Loads qmtc32.dll or qmtc64.dll into process of hwnd, and sets hooks.
 This function is rarely used explicitly. Capture() and other functions implicitly calls it if need.
 Only single variable can capture text at a time. Variables of other threads and processes will wait. Here "capture text" means the time between Begin() called and End() called, explicitly or implicitly.


if(!m_hwnd) end ERR_INIT
if(!m_tc) m_tc=CreateTextCapture

m_tc.Begin(m_hwnd)

err+ end _error
