 /
function#

 Gets screen width, in pixels.

 REMARKS
 There may be multiple monitors. This function gets width of the primary monitor.
 To get location of other monitors, use MonitorFromIndex or GetWorkArea.


ret GetSystemMetrics(SM_CXSCREEN)
