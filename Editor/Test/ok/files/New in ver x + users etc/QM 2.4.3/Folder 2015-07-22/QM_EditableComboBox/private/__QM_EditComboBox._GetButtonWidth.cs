function#
 Returns button width from the right edge of the window.

if(!_themeInited) _themeInited=1; _Theme(WM_CREATE)
int R=GetSystemMetrics(SM_CXVSCROLL)
if(!_theme) R+_borderWidth; else if(!_borderWidth) R-1; else if(_winver<0x600) R+1
ret R

 tested: it seems that combo button width is always calculated from SM_CXVSCROLL. GetThemePartSize gets inorrect results. All other theme metrics API fail or get 0.
 RECT k kk; SIZE z
 if(!GetThemePartSize(_theme dc CP_DROPDOWNBUTTON CBXS_NORMAL &r TS_TRUE &z)) out "%i %i" z.cx z.cy ;;7 21
 if(!GetThemePartSize(_theme dc CP_DROPDOWNBUTTONLEFT CBXS_NORMAL &r TS_TRUE &z)) out "%i %i" z.cx z.cy ;;6 23
 if(!GetThemePartSize(_theme dc CP_DROPDOWNBUTTONRIGHT CBXS_NORMAL &r TS_TRUE &z)) out "%i %i" z.cx z.cy ;;5 23
 if(!GetThemeBackgroundContentRect(_theme dc CP_DROPDOWNBUTTON CBXS_NORMAL &r &k)) outRECT k ;;144 20
 if(!GetThemeTransitionDuration(_theme CP_DROPDOWNBUTTON CBXS_NORMAL CBB_HOT TMT_TRANSITIONDURATIONS &_i)) out _i ;;225
  these fail
 if(!GetThemeRect(_theme CP_DROPDOWNBUTTON CBXS_NORMAL TMT_RECT &k)) outRECT k
 if(!GetThemeMetric(_theme dc CP_DROPDOWNBUTTON CBXS_NORMAL TMT_WIDTH &_i)) out _i
