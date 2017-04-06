function! $imageFile [GDIP.EncoderParameters*ep]

 Saves this image to file.
 Supported formats: BMP, GIF, JPEG, PNG, TIFF.
 Returns 1 on success, 0 if failed.
 Cannot save to the same file from which FromFile'ed.


str s1.GetFilenameExt(imageFile); s1.lcase
sel(s1) case "jpg" s1="jpeg"; case "tif" s1="tiff"
s1-"image/"
GUID clsEncoder
if(!__GdipGetEncoderClsid(s1 clsEncoder)) ret

_hresult=GDIP.GdipSaveImageToFile(m_i @s1.expandpath(imageFile) &clsEncoder ep)
ret !_hresult
