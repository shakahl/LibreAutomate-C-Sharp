function! row mask [param] [image] [overlayImage] [stateImage] [indent] ;;mask: 1 param, 2 image, 4 overlayImage, 8 stateImage, 16 indent

 Sets list view control item properties.
 Returns: 1 success, 0 failed.

 row - 0-based row index. Same as with <help>DlgGrid.RowAddSetMS</help>.
 mask - what properties to change. Flags. For example, if you want to set image and overlay image, use mask 2|4.
 other parameters - see <help #IDP_QMGRID#details>grid row properties</help>.


LVITEM li
if(mask&1) li.mask|LVIF_PARAM; li.lParam=param
if(mask&2) li.mask|LVIF_IMAGE; li.iImage=iif(image<0 -2 image)
if(mask&4) li.mask|LVIF_STATE; li.stateMask|0xF00; li.state|overlayImage&15<<8
if(mask&8) li.mask|LVIF_STATE; li.stateMask|0xF000; li.state|stateImage&15<<12
if(mask&16) li.mask|LVIF_INDENT; li.iIndent=indent

ret RowAddSetMS(row 0 0 0 1 &li)>=0
