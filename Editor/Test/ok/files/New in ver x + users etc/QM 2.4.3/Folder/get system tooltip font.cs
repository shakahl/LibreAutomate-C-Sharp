NONCLIENTMETRICS m.cbSize=sizeof(m)
SystemParametersInfo(SPI_GETNONCLIENTMETRICS m.cbSize &m 0)
LOGFONT& p=&m.lfStatusFont
int hdc=GetDC(0); out -MulDiv(p.lfHeight 72 GetDeviceCaps(hdc LOGPIXELSY)); ReleaseDC 0 hdc
str s.fromn(&p.lfFaceName -1); out s
