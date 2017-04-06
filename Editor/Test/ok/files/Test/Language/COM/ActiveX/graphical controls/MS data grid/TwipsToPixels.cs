 /
function# twips vertical

int dc=GetDC(0)
twips/1440/GetDeviceCaps(dc iif(vertical LOGPIXELSY LOGPIXELSX))
ReleaseDC 0 dc
ret twips
