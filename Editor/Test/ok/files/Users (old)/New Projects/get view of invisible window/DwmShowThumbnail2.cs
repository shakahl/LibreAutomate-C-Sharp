 /dlg_window_thumbnail
function hwnd hwnd2

 Works well, but on Vista only.

 ref WINAPI2
dll dwmapi #DwmRegisterThumbnail hwndDestination hwndSource *phThumbnailId
type DWM_THUMBNAIL_PROPERTIES dwFlags RECT'rcDestination RECT'rcSource !opacity fVisible fSourceClientAreaOnly
def DWM_TNP_RECTSOURCE 0x00000002
def DWM_TNP_RECTDESTINATION 0x00000001
def DWM_TNP_VISIBLE 0x00000008
dll dwmapi #DwmUpdateThumbnailProperties hThumbnailId DWM_THUMBNAIL_PROPERTIES*ptnProperties

int thumbnail
int hr = DwmRegisterThumbnail(hwnd, hwnd2, &thumbnail);
if(hr) ret

//destination rectangle size
RECT dest; GetClientRect hwnd &dest

//Set thumbnail properties for use
DWM_THUMBNAIL_PROPERTIES dskThumbProps;
 dskThumbProps.dwFlags = DWM_TNP_RECTDESTINATION | DWM_TNP_VISIBLE ;;| DWM_TNP_SOURCECLIENTAREAONLY; ;;note: if DWM_TNP_SOURCECLIENTAREAONLY flag set, always gets only client regardless of fSourceClientAreaOnly.
dskThumbProps.dwFlags = DWM_TNP_RECTSOURCE|DWM_TNP_RECTDESTINATION | DWM_TNP_VISIBLE ;;| DWM_TNP_SOURCECLIENTAREAONLY; ;;note: if DWM_TNP_SOURCECLIENTAREAONLY flag set, always gets only client regardless of fSourceClientAreaOnly.
//use window frame and client area
dskThumbProps.fSourceClientAreaOnly = 1;
dskThumbProps.fVisible = 1;
dskThumbProps.opacity = 255;
dskThumbProps.rcDestination = dest;
dskThumbProps.rcSource = dest;

//display the thumbnail
hr = DwmUpdateThumbnailProperties(thumbnail,&dskThumbProps);
