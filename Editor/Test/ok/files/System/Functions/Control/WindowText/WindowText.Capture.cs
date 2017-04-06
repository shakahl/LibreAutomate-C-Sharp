function [addFlags]

 Captures window text.

 addFlags - additional text capturing flags. See <help>WindowText.Init</help>. 

 REMARKS
 Stores properties of captured text items in internal array. It is member a of this variable. Member n is number of elements.
 You can access the array directly, or use Find() to find an item in it.
 The array becomes invalid when the variable is destroyed or when a text capturing function called again.
 You can call Capture() multiple times.
 Init() must be called to set target window.
 Cannot capture text if the window is minimized or hidden. Not error, but gets 0 results.
 This function is called by Find(), Wait() and CaptureToString(). You will probably rarely call this function explicitly. Call it when you don't need to find, wait etc, but just need array of text items.

 EXAMPLE
 out
 int w=win("" "QM_Editor")
 WindowText x.Init(w)
 x.Capture
 int i
 for i 0 x.n
	 out x.a[i].txt


if(!m_hwnd) end ERR_INIT
if(!m_tc) m_tc=CreateTextCapture

a=0; n=0; m_captured=0
n=m_tc.Capture(a m_hwnd m_flags|addFlags m_rp)
m_captured=1

if m_dpiScaled
	int i
	for i 0 this.n
		 outw a[i].hwnd
		DpiScale +&a[i].rt 4
		 zRECT a[i].rv

err+ end _error
