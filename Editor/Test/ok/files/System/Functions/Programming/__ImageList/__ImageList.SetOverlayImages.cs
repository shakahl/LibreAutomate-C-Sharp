function $images

 Sets images that can be used as overlay images.

 images - space-separated list of images that will be used as overlay images.
	 For example, "0 1 8 9" sets images 0, 1, 8, 9 to be used as overlay images 1, 2, 3, 4.

 Added in: QM 2.3.2.


ARRAY(lpstr) a
int i nt im
nt=tok(images a -1 1)
for i 0 nt
	im=val(a[i] 0 _i)
	if(_i) ImageList_SetOverlayImage(handle im i+1)
