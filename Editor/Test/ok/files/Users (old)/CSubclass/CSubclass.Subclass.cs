function hWnd newWndProc [flags] ;;flags: destructor destroys window

 Subclasses window hWnd.
 Subclassing is automatically removed when the object goes out of scope.
 Especially useful to subclass windows that belong to the QM main thread,
 because they are not destroyed when [re]loading file. If you don't
 unsubclass them when [re]loading, you'll get "invalid callback" errors.


if(m_hwnd) UnSubclass
m_wndproc=SubclassWindow(hWnd newWndProc)
m_hwnd=hWnd
m_flags=flags
