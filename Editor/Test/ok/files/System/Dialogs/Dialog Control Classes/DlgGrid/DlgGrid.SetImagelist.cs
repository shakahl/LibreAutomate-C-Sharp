function [ilNormal] [ilState]

 Sets imagelists.

 ilNormal - imagelist for normal images and overlay images. Can be 0 if don't need to set.
 ilState - imagelist for state images. Can be 0 if don't need to set. Can be same as ilNormal.

 Imagelists can be created with QM Imagelist Editor. Icon size can be 16 or other.
 To load an imagelist, declare thread variable of type __ImageList and call its function Load (imagelist editor can create the code). Or call Create to create from icons. Then pass the variable to this function.
 The grid control does not copy or destroy the imagelist. The __ImageList variable manages it, and therefore must not die earlier than the control.
 If need overlay images, call SetOverlayImages function of the normal imagelist variable.


if(ilNormal) Send(LVM_SETIMAGELIST LVSIL_SMALL ilNormal)
if(ilState) Send(LVM_SETIMAGELIST LVSIL_STATE ilState)
