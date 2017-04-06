function! row [int&param] [int&image] [int&overlayImage] [int&stateImage] [int&indent]

 Gets list view control item properties.
 Returns: 1 success, 0 failed.

 row - 0-based row index.
 other parameters - int variables that receive properties. Can be 0. See <help #IDP_QMGRID#details>grid row properties</help>.


LVITEM li
if(&param) li.mask|LVIF_PARAM
if(&image) li.mask|LVIF_IMAGE
if(&overlayImage) li.mask|LVIF_STATE; li.stateMask|0xF00
if(&stateImage) li.mask|LVIF_STATE; li.stateMask|0xF000
if(&indent) li.mask|LVIF_INDENT

if(!RowGetMS(row 0 0 0 0 &li)) ret

if(&param) param=li.lParam
if(&image) image=li.iImage
if(&overlayImage); overlayImage=li.state>>8&15
if(&stateImage); stateImage=li.state>>12&15
if(&indent) indent=li.iIndent

ret 1
