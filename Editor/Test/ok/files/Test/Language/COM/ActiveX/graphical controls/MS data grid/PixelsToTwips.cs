 /
function# pixels vertical

int dc=GetDC(0)
out 1440/GetDeviceCaps(dc iif(vertical LOGPIXELSY LOGPIXELSX))
pixels*1440/GetDeviceCaps(dc iif(vertical LOGPIXELSY LOGPIXELSX))
ReleaseDC 0 dc
ret pixels
