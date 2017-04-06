function hWnd RECT&r [hParent]

GetWindowRect hWnd &r
if(hParent) MapWindowPoints 0 hParent +&r 2
SwapRECT(r)
