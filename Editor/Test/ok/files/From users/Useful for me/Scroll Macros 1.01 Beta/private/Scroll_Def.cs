def WM_MOUSEWHEEL 0x020A
def WHEEL_DELTA 120
def WM_VSCROLL 0x115
def SB_LINEDOWN 1
def SB_LINEUP 0
def SB_PAGEUP        2
def SB_PAGEDOWN      3
def SPI_GETWHEELSCROLLLINES  104
type MOUSESCROLL scrollFactor deltaFactor setScrollFactor setDeltaFactor forceScroll lineDefault
 type MOUSESCROLL
 scrollFactor    - Count of lines or pages to scroll by
 deltaFactor     - A multiple of WHEEL_DELTA, used for WM_MOUSEWHEEL message
 setScrollFactor - If set to 1, then user is asked for a new scrollFactor before scrolling
 setDeltaFactor  - If set to 1, then user is asked for a new deltaFactor before scrolling
 forceScroll     - 0 - normal (sends WM_MOUSEWHEEL or WM_VSCROLL as charted in the Needs_WM_MOUSEWHEEL function)
                 - 1 - use WM_MOUSEWHEEL for all windows
                 - 2 - use WM_VSCROLL for all windows
MOUSESCROLL+ _m
