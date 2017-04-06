 Initialization for the set of Scroll Macros

#compile Scroll_Def

MOUSESCROLL+ _m

 type MOUSESCROLL:
 scrollFactor    - Count of lines or pages to scroll by
 deltaFactor     - A multiple of WHEEL_DELTA, used for WM_MOUSEWHEEL message
 setScrollFactor - If set to 1, then user is asked for a new scrollFactor before scrolling
 setDeltaFactor  - If set to 1, then user is asked for a new deltaFactor before scrolling
 forceScroll     - 0 - normal (sends WM_MOUSEWHEEL or WM_VSCROLL as charted in the Needs_WM_MOUSEWHEEL function)
                 - 1 - use WM_MOUSEWHEEL for all windows
                 - 2 - use WM_VSCROLL for all windows

  Defaults
if(!SystemParametersInfo(SPI_GETWHEELSCROLLLINES 0 &_m.lineDefault 0)) _m.lineDefault=3
_m.forceScroll=1
