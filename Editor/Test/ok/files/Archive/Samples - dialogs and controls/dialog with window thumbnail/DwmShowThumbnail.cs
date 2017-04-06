 /
function# hwnd hwnd2

 Shows thumbnail of hwnd2 in hwnd.
 Works on Vista/7+ only.


if(_winnt<6) ret

type DWM_THUMBNAIL_PROPERTIES dwFlags RECT'rcDestination RECT'rcSource !opacity fVisible fSourceClientAreaOnly
def DWM_TNP_RECTDESTINATION 0x00000001
def DWM_TNP_RECTSOURCE 0x00000002
def DWM_TNP_OPACITY 0x00000004
def DWM_TNP_VISIBLE 0x00000008
def DWM_TNP_SOURCECLIENTAREAONLY 0x00000010
dll- dwmapi
	#DwmRegisterThumbnail hwndDestination hwndSource *phThumbnailId
	#DwmUnregisterThumbnail hThumbnailId
	#DwmUpdateThumbnailProperties hThumbnailId DWM_THUMBNAIL_PROPERTIES*ptnProperties

int thumbnail
int hr = DwmRegisterThumbnail(hwnd hwnd2 &thumbnail)
if(hr) ret

RECT r; GetClientRect hwnd &r

DWM_THUMBNAIL_PROPERTIES tp
tp.dwFlags = DWM_TNP_RECTDESTINATION|DWM_TNP_VISIBLE|DWM_TNP_SOURCECLIENTAREAONLY ;;note: if DWM_TNP_SOURCECLIENTAREAONLY flag set, always gets only client regardless of fSourceClientAreaOnly.
tp.fSourceClientAreaOnly = 1
tp.fVisible = 1
tp.opacity = 255
tp.rcDestination = r

hr = DwmUpdateThumbnailProperties(thumbnail &tp)

ret thumbnail
