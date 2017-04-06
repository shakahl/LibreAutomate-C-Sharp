function [flags]

 Same as Init(), but allows user to select target window and rectangle at run time.
 Error if user cancels window selection with Esc.

 flags - same as with <help>WindowText.Init</help>.


int hwnd; RECT r; RECT* rp
if(CaptureWindowAndRect(hwnd r)=2) rp=&r
Init(hwnd rp flags)

err+ end _error
