 /
function NMLVDISPINFOW&di $text [imageIndex] [overlayImageIndex] [stateImageIndex] [indent]

 Call when handling listview LVN_GETDISPINFOW notification. Sets item text, image etc if need.
 You could easily fill NMLVDISPINFOW, but need an UTF-16 string, and it cannot be a local variable, would need to store it somewhere, etc.
 This function does not set subitem index, the caller must set it if need.
 Don't use for QM grid control.

__DispinfoText-- t
LVITEMW& li=di.item
if(li.mask&LVIF_TEXT) li.pszText=t.Get(text)
if(li.mask&LVIF_IMAGE) li.iImage=imageIndex
if overlayImageIndex or stateImageIndex
	if(!(li.mask&LVIF_STATE)) li.mask|=LVIF_STATE; li.state=0; li.stateMask=0 ;;messed on XP
	if(overlayImageIndex) li.stateMask|=LVIS_OVERLAYMASK; li.state=li.state~LVIS_OVERLAYMASK|(overlayImageIndex&15<<8)
	if(stateImageIndex) li.stateMask|=LVIS_STATEIMAGEMASK; li.state=li.state~LVIS_STATEIMAGEMASK|(stateImageIndex&15<<12)
if(li.mask&LVIF_INDENT) li.iIndent=indent
