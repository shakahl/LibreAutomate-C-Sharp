function hwnd

 Gets top-level Java accessible object of Java window.
 Error if the window does not implement the Java Accessibility API, or Java Access Bridge is disabled.


opt noerrorshere 1
JavaAO_JabInit
Clear
if(!JAB.GetAccessibleContextFromHWND(hwnd &vmID &a) or !a) end ERR_FAILED
