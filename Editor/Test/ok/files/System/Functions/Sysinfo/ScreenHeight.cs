 /
function#

 Gets screen height, in pixels.

 REMARKS
 There may be multiple monitors. This function gets height of the primary monitor.
 To get location of other monitors, use MonitorFromIndex or GetWorkArea.


ret GetSystemMetrics(SM_CYSCREEN)
