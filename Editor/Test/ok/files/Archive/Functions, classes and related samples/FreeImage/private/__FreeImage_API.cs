#ret
def BMP_DEFAULT 0
def BMP_SAVE_RLE 1
def CUT_DEFAULT 0
def DDS_DEFAULT 0
def EXR_B44 0x0020
def EXR_DEFAULT 0
def EXR_FLOAT 0x0001
def EXR_LC 0x0040
def EXR_NONE 0x0002
def EXR_PIZ 0x0008
def EXR_PXR24 0x0010
def EXR_ZIP 0x0004
def FAXG3_DEFAULT 0
def FI16_555_BLUE_MASK 0x001F
def FI16_555_BLUE_SHIFT 0
def FI16_555_GREEN_MASK 0x03E0
def FI16_555_GREEN_SHIFT 5
def FI16_555_RED_MASK 0x7C00
def FI16_555_RED_SHIFT 10
def FI16_565_BLUE_MASK 0x001F
def FI16_565_BLUE_SHIFT 0
def FI16_565_GREEN_MASK 0x07E0
def FI16_565_GREEN_SHIFT 5
def FI16_565_RED_MASK 0xF800
def FI16_565_RED_SHIFT 11
type FIBITMAP !*data
def FICC_ALPHA 4
def FICC_BLACK 5
def FICC_BLUE 3
def FICC_GREEN 2
def FICC_IMAG 7
def FICC_MAG 8
def FICC_PHASE 9
def FICC_REAL 6
def FICC_RED 1
def FICC_RGB 0
type FICOMPLEX ^r ^i [pack1]
def FIC_CMYK 5
def FIC_MINISBLACK 1
def FIC_MINISWHITE 0
def FIC_PALETTE 3
def FIC_RGB 2
def FIC_RGBALPHA 4
def FIDT_ASCII 2
def FIDT_BYTE 1
def FIDT_DOUBLE 12
def FIDT_FLOAT 11
def FIDT_IFD 13
def FIDT_IFD8 18
def FIDT_LONG 4
def FIDT_LONG8 16
def FIDT_NOTYPE 0
def FIDT_PALETTE 14
def FIDT_RATIONAL 5
def FIDT_SBYTE 6
def FIDT_SHORT 3
def FIDT_SLONG 9
def FIDT_SLONG8 17
def FIDT_SRATIONAL 10
def FIDT_SSHORT 8
def FIDT_UNDEFINED 7
def FID_BAYER16x16 6
def FID_BAYER4x4 1
def FID_BAYER8x8 2
def FID_CLUSTER16x16 5
def FID_CLUSTER6x6 3
def FID_CLUSTER8x8 4
def FID_FS 0
def FIF_BMP 0
def FIF_CUT 21
def FIF_DDS 24
def FIF_EXR 29
def FIF_FAXG3 27
def FIF_GIF 25
def FIF_HDR 26
def FIF_ICO 1
def FIF_IFF 5
def FIF_J2K 30
def FIF_JNG 3
def FIF_JP2 31
def FIF_JPEG 2
def FIF_KOALA 4
def FIF_LBM 5
def FIF_LOAD_NOPIXELS 0x8000
def FIF_MNG 6
def FIF_PBM 7
def FIF_PBMRAW 8
def FIF_PCD 9
def FIF_PCX 10
def FIF_PFM 32
def FIF_PGM 11
def FIF_PGMRAW 12
def FIF_PICT 33
def FIF_PNG 13
def FIF_PPM 14
def FIF_PPMRAW 15
def FIF_PSD 20
def FIF_RAS 16
def FIF_RAW 34
def FIF_SGI 28
def FIF_TARGA 17
def FIF_TIFF 18
def FIF_UNKNOWN 0xFFFFFFFF
def FIF_WBMP 19
def FIF_XBM 22
def FIF_XPM 23
type FIICCPROFILE @flags size !*data
def FIICC_COLOR_IS_CMYK 0x01
def FIICC_DEFAULT 0x00
def FIJPEG_OP_FLIP_H 1
def FIJPEG_OP_FLIP_V 2
def FIJPEG_OP_NONE 0
def FIJPEG_OP_ROTATE_180 6
def FIJPEG_OP_ROTATE_270 7
def FIJPEG_OP_ROTATE_90 5
def FIJPEG_OP_TRANSPOSE 3
def FIJPEG_OP_TRANSVERSE 4
def FILTER_BICUBIC 1
def FILTER_BILINEAR 2
def FILTER_BOX 0
def FILTER_BSPLINE 3
def FILTER_CATMULLROM 4
def FILTER_LANCZOS3 5
def FIMD_ANIMATION 9
def FIMD_COMMENTS 0
def FIMD_CUSTOM 10
def FIMD_EXIF_EXIF 2
def FIMD_EXIF_GPS 3
def FIMD_EXIF_INTEROP 5
def FIMD_EXIF_MAIN 1
def FIMD_EXIF_MAKERNOTE 4
def FIMD_EXIF_RAW 11
def FIMD_GEOTIFF 8
def FIMD_IPTC 6
def FIMD_NODATA 0xFFFFFFFF
def FIMD_XMP 7
type FIMEMORY !*data
type FIMETADATA !*data
type FIMULTIBITMAP !*data
def FIQ_NNQUANT 1
def FIQ_WUQUANT 0
type FIRGB16 @red @green @blue [pack1]
type FIRGBA16 @red @green @blue @alpha [pack1]
type FIRGBAF FLOAT'red FLOAT'green FLOAT'blue FLOAT'alpha [pack1]
type FIRGBF FLOAT'red FLOAT'green FLOAT'blue [pack1]
type FITAG !*data
def FITMO_DRAGO03 0
def FITMO_FATTAL02 2
def FITMO_REINHARD05 1
def FIT_BITMAP 1
def FIT_COMPLEX 8
def FIT_DOUBLE 7
def FIT_FLOAT 6
def FIT_INT16 3
def FIT_INT32 5
def FIT_RGB16 9
def FIT_RGBA16 10
def FIT_RGBAF 12
def FIT_RGBF 11
def FIT_UINT16 2
def FIT_UINT32 4
def FIT_UNKNOWN 0
def FI_COLOR_ALPHA_IS_INDEX 0x04
def FI_COLOR_FIND_EQUAL_COLOR 0x02
def FI_COLOR_IS_RGBA_COLOR 0x01
def FI_COLOR_IS_RGB_COLOR 0x00
def FI_COLOR_PALETTE_SEARCH_MASK 0x00000006
type FI_CloseProc = #
 ;;function FreeImageIO*io !*handle !*data
dll C_macro FI_DEFAULT x
 ;;=x
type FI_DescriptionProc = #
 ;;function$
dll C_macro FI_ENUM x
 ;;enum x
type FI_ExtensionListProc = #
 ;;function$
type FI_FormatProc = #
 ;;function$
type FI_InitProc = #
 ;;function Plugin*plugin format_id
type FI_LoadProc = #
 ;;function'FIBITMAP* FreeImageIO*io !*handle page flags !*data
type FI_MimeProc = #
 ;;function$
type FI_OpenProc = #
 ;;function!* FreeImageIO*io !*handle read
type FI_PageCapabilityProc = #
 ;;function# FreeImageIO*io !*handle !*data
type FI_PageCountProc = #
 ;;function# FreeImageIO*io !*handle !*data
def FI_RGBA_ALPHA 3
def FI_RGBA_ALPHA_MASK 0x000000FF
def FI_RGBA_ALPHA_SHIFT 0
def FI_RGBA_BLUE 0
def FI_RGBA_BLUE_MASK 0xFF000000
def FI_RGBA_BLUE_SHIFT 24
def FI_RGBA_GREEN 1
def FI_RGBA_GREEN_MASK 0x00FF0000
def FI_RGBA_GREEN_SHIFT 16
def FI_RGBA_RED 2
def FI_RGBA_RED_MASK 0x0000FF00
def FI_RGBA_RED_SHIFT 8
def FI_RGBA_RGB_MASK 0xFFFFFF00
type FI_RegExprProc = #
 ;;function$
dll C_macro FI_STRUCT x
 ;;struct x
type FI_SaveProc = #
 ;;function# FreeImageIO*io FIBITMAP*dib !*handle page flags !*data
type FI_SeekProc = #
 ;;function# !*handle offset origin
type FI_SupportsExportBPPProc = #
 ;;function# bpp
type FI_SupportsExportTypeProc = #
 ;;function# type
type FI_SupportsICCProfilesProc = #
 ;;function#
type FI_SupportsNoPixelsProc = #
 ;;function#
type FI_TellProc = #
 ;;function# !*handle
type FI_ValidateProc = #
 ;;function# FreeImageIO*io !*handle
type FI_ReadProc = #
 ;;function# !*buffer size count !*handle
type FI_WriteProc = #
 ;;function# !*buffer size count !*handle
def FREEIMAGE_BIGENDIAN
def FREEIMAGE_COLORORDER 1
def FREEIMAGE_COLORORDER_BGR 0
def FREEIMAGE_COLORORDER_RGB 1
def FREEIMAGE_H
def FREEIMAGE_IO
def FREEIMAGE_MAJOR_VERSION 3
def FREEIMAGE_MINOR_VERSION 15
def FREEIMAGE_RELEASE_SERIAL 4
type FREE_IMAGE_COLOR_CHANNEL = #
type FREE_IMAGE_COLOR_TYPE = #
type FREE_IMAGE_DITHER = #
type FREE_IMAGE_FILTER = #
type FREE_IMAGE_FORMAT = #
type FREE_IMAGE_JPEG_OPERATION = #
type FREE_IMAGE_MDMODEL = #
type FREE_IMAGE_MDTYPE = #
type FREE_IMAGE_QUANTIZE = #
type FREE_IMAGE_TMO = #
type FREE_IMAGE_TYPE = #
type FreeImageIO FI_ReadProc'read_proc FI_WriteProc'write_proc seek_proc tell_proc [pack1]
dll- "$qm$\FreeImage" [_FreeImage_AcquireMemory@12]#FreeImage_AcquireMemory FIMEMORY*stream !**data *size_in_bytes
dll- "$qm$\FreeImage" [_FreeImage_AdjustBrightness@12]#FreeImage_AdjustBrightness FIBITMAP*dib ^percentage
dll- "$qm$\FreeImage" [_FreeImage_AdjustColors@32]#FreeImage_AdjustColors FIBITMAP*dib ^brightness ^contrast ^gamma invert
dll- "$qm$\FreeImage" [_FreeImage_AdjustContrast@12]#FreeImage_AdjustContrast FIBITMAP*dib ^percentage
dll- "$qm$\FreeImage" [_FreeImage_AdjustCurve@12]#FreeImage_AdjustCurve FIBITMAP*dib !*LUT channel
dll- "$qm$\FreeImage" [_FreeImage_AdjustGamma@12]#FreeImage_AdjustGamma FIBITMAP*dib ^gamma
dll- "$qm$\FreeImage" [_FreeImage_Allocate@24]FIBITMAP*FreeImage_Allocate width height bpp red_mask green_mask blue_mask
dll- "$qm$\FreeImage" [_FreeImage_AllocateEx@36]FIBITMAP*FreeImage_AllocateEx width height bpp RGBQUAD*color options RGBQUAD*palette red_mask green_mask blue_mask
dll- "$qm$\FreeImage" [_FreeImage_AllocateExT@40]FIBITMAP*FreeImage_AllocateExT type width height bpp !*color options RGBQUAD*palette red_mask green_mask blue_mask
dll- "$qm$\FreeImage" [_FreeImage_AllocateT@28]FIBITMAP*FreeImage_AllocateT type width height bpp red_mask green_mask blue_mask
dll- "$qm$\FreeImage" [_FreeImage_AppendPage@8]FreeImage_AppendPage FIMULTIBITMAP*bitmap FIBITMAP*data
dll- "$qm$\FreeImage" [_FreeImage_ApplyColorMapping@24]#FreeImage_ApplyColorMapping FIBITMAP*dib RGBQUAD*srccolors RGBQUAD*dstcolors count'a ignore_alpha swap
dll- "$qm$\FreeImage" [_FreeImage_ApplyPaletteIndexMapping@20]#FreeImage_ApplyPaletteIndexMapping FIBITMAP*dib !*srcindices !*dstindices count'a swap
dll- "$qm$\FreeImage" [_FreeImage_Clone@4]FIBITMAP*FreeImage_Clone FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_CloneMetadata@8]#FreeImage_CloneMetadata FIBITMAP*dst FIBITMAP*src
dll- "$qm$\FreeImage" [_FreeImage_CloneTag@4]FITAG*FreeImage_CloneTag FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_CloseMemory@4]FreeImage_CloseMemory FIMEMORY*stream
dll- "$qm$\FreeImage" [_FreeImage_CloseMultiBitmap@8]#FreeImage_CloseMultiBitmap FIMULTIBITMAP*bitmap flags
dll- "$qm$\FreeImage" [_FreeImage_ColorQuantize@8]FIBITMAP*FreeImage_ColorQuantize FIBITMAP*dib quantize
dll- "$qm$\FreeImage" [_FreeImage_ColorQuantizeEx@20]FIBITMAP*FreeImage_ColorQuantizeEx FIBITMAP*dib quantize PaletteSize ReserveSize RGBQUAD*ReservePalette
dll- "$qm$\FreeImage" [_FreeImage_Composite@16]FIBITMAP*FreeImage_Composite FIBITMAP*fg useFileBkg RGBQUAD*appBkColor FIBITMAP*bg
dll- "$qm$\FreeImage" [_FreeImage_ConvertFromRawBits@36]FIBITMAP*FreeImage_ConvertFromRawBits !*bits width height pitch bpp'a red_mask green_mask blue_mask topdown
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16To24_555@12]FreeImage_ConvertLine16To24_555 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16To24_565@12]FreeImage_ConvertLine16To24_565 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16To32_555@12]FreeImage_ConvertLine16To32_555 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16To32_565@12]FreeImage_ConvertLine16To32_565 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16To4_555@12]FreeImage_ConvertLine16To4_555 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16To4_565@12]FreeImage_ConvertLine16To4_565 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16To8_555@12]FreeImage_ConvertLine16To8_555 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16To8_565@12]FreeImage_ConvertLine16To8_565 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16_555_To16_565@12]FreeImage_ConvertLine16_555_To16_565 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine16_565_To16_555@12]FreeImage_ConvertLine16_565_To16_555 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine1To16_555@16]FreeImage_ConvertLine1To16_555 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine1To16_565@16]FreeImage_ConvertLine1To16_565 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine1To24@16]FreeImage_ConvertLine1To24 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine1To32@16]FreeImage_ConvertLine1To32 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine1To4@12]FreeImage_ConvertLine1To4 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine1To8@12]FreeImage_ConvertLine1To8 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine24To16_555@12]FreeImage_ConvertLine24To16_555 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine24To16_565@12]FreeImage_ConvertLine24To16_565 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine24To32@12]FreeImage_ConvertLine24To32 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine24To4@12]FreeImage_ConvertLine24To4 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine24To8@12]FreeImage_ConvertLine24To8 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine32To16_555@12]FreeImage_ConvertLine32To16_555 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine32To16_565@12]FreeImage_ConvertLine32To16_565 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine32To24@12]FreeImage_ConvertLine32To24 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine32To4@12]FreeImage_ConvertLine32To4 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine32To8@12]FreeImage_ConvertLine32To8 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine4To16_555@16]FreeImage_ConvertLine4To16_555 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine4To16_565@16]FreeImage_ConvertLine4To16_565 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine4To24@16]FreeImage_ConvertLine4To24 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine4To32@16]FreeImage_ConvertLine4To32 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine4To8@12]FreeImage_ConvertLine4To8 !*target !*source width_in_pixels
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine8To16_555@16]FreeImage_ConvertLine8To16_555 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine8To16_565@16]FreeImage_ConvertLine8To16_565 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine8To24@16]FreeImage_ConvertLine8To24 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine8To32@16]FreeImage_ConvertLine8To32 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertLine8To4@16]FreeImage_ConvertLine8To4 !*target !*source width_in_pixels RGBQUAD*palette
dll- "$qm$\FreeImage" [_FreeImage_ConvertTo16Bits555@4]FIBITMAP*FreeImage_ConvertTo16Bits555 FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertTo16Bits565@4]FIBITMAP*FreeImage_ConvertTo16Bits565 FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertTo24Bits@4]FIBITMAP*FreeImage_ConvertTo24Bits FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertTo32Bits@4]FIBITMAP*FreeImage_ConvertTo32Bits FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertTo4Bits@4]FIBITMAP*FreeImage_ConvertTo4Bits FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertTo8Bits@4]FIBITMAP*FreeImage_ConvertTo8Bits FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertToFloat@4]FIBITMAP*FreeImage_ConvertToFloat FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertToGreyscale@4]FIBITMAP*FreeImage_ConvertToGreyscale FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertToRGB16@4]FIBITMAP*FreeImage_ConvertToRGB16 FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertToRGBF@4]FIBITMAP*FreeImage_ConvertToRGBF FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ConvertToRawBits@32]FreeImage_ConvertToRawBits !*bits FIBITMAP*dib pitch bpp'a red_mask green_mask blue_mask topdown
dll- "$qm$\FreeImage" [_FreeImage_ConvertToStandardType@8]FIBITMAP*FreeImage_ConvertToStandardType FIBITMAP*src scale_linear
dll- "$qm$\FreeImage" [_FreeImage_ConvertToType@12]FIBITMAP*FreeImage_ConvertToType FIBITMAP*src dst_type scale_linear
dll- "$qm$\FreeImage" [_FreeImage_ConvertToUINT16@4]FIBITMAP*FreeImage_ConvertToUINT16 FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_Copy@20]FIBITMAP*FreeImage_Copy FIBITMAP*dib left top right bottom
dll- "$qm$\FreeImage" [_FreeImage_CreateICCProfile@12]FIICCPROFILE*FreeImage_CreateICCProfile FIBITMAP*dib !*data size
dll- "$qm$\FreeImage" [_FreeImage_CreateTag@0]FITAG*FreeImage_CreateTag
dll- "$qm$\FreeImage" [_FreeImage_DeInitialise@0]FreeImage_DeInitialise
dll- "$qm$\FreeImage" [_FreeImage_DeletePage@8]FreeImage_DeletePage FIMULTIBITMAP*bitmap page
dll- "$qm$\FreeImage" [_FreeImage_DeleteTag@4]FreeImage_DeleteTag FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_DestroyICCProfile@4]FreeImage_DestroyICCProfile FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_Dither@8]FIBITMAP*FreeImage_Dither FIBITMAP*dib algorithm
dll- "$qm$\FreeImage" [_FreeImage_EnlargeCanvas@28]FIBITMAP*FreeImage_EnlargeCanvas FIBITMAP*src left top right bottom !*color options
dll- "$qm$\FreeImage" [_FreeImage_FIFSupportsExportBPP@8]#FreeImage_FIFSupportsExportBPP fif bpp
dll- "$qm$\FreeImage" [_FreeImage_FIFSupportsExportType@8]#FreeImage_FIFSupportsExportType fif type
dll- "$qm$\FreeImage" [_FreeImage_FIFSupportsICCProfiles@4]#FreeImage_FIFSupportsICCProfiles fif
dll- "$qm$\FreeImage" [_FreeImage_FIFSupportsNoPixels@4]#FreeImage_FIFSupportsNoPixels fif
dll- "$qm$\FreeImage" [_FreeImage_FIFSupportsReading@4]#FreeImage_FIFSupportsReading fif
dll- "$qm$\FreeImage" [_FreeImage_FIFSupportsWriting@4]#FreeImage_FIFSupportsWriting fif
dll- "$qm$\FreeImage" [_FreeImage_FillBackground@12]#FreeImage_FillBackground FIBITMAP*dib !*color options
dll- "$qm$\FreeImage" [_FreeImage_FindCloseMetadata@4]FreeImage_FindCloseMetadata FIMETADATA*mdhandle
dll- "$qm$\FreeImage" [_FreeImage_FindFirstMetadata@12]FIMETADATA*FreeImage_FindFirstMetadata model FIBITMAP*dib FITAG**tag
dll- "$qm$\FreeImage" [_FreeImage_FindNextMetadata@8]#FreeImage_FindNextMetadata FIMETADATA*mdhandle FITAG**tag
dll- "$qm$\FreeImage" [_FreeImage_FlipHorizontal@4]#FreeImage_FlipHorizontal FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_FlipVertical@4]#FreeImage_FlipVertical FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetAdjustColorsLookupTable@32]#FreeImage_GetAdjustColorsLookupTable !*LUT ^brightness ^contrast ^gamma invert
dll- "$qm$\FreeImage" [_FreeImage_GetBPP@4]#FreeImage_GetBPP FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetBackgroundColor@8]#FreeImage_GetBackgroundColor FIBITMAP*dib RGBQUAD*bkcolor
dll- "$qm$\FreeImage" [_FreeImage_GetBits@4]!*FreeImage_GetBits FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetBlueMask@4]#FreeImage_GetBlueMask FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetChannel@8]FIBITMAP*FreeImage_GetChannel FIBITMAP*dib channel
dll- "$qm$\FreeImage" [_FreeImage_GetColorType@4]#FreeImage_GetColorType FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetColorsUsed@4]#FreeImage_GetColorsUsed FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetComplexChannel@8]FIBITMAP*FreeImage_GetComplexChannel FIBITMAP*src channel
dll- "$qm$\FreeImage" [_FreeImage_GetCopyrightMessage@0]$FreeImage_GetCopyrightMessage
dll- "$qm$\FreeImage" [_FreeImage_GetDIBSize@4]#FreeImage_GetDIBSize FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetDotsPerMeterX@4]#FreeImage_GetDotsPerMeterX FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetDotsPerMeterY@4]#FreeImage_GetDotsPerMeterY FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetFIFCount@0]#FreeImage_GetFIFCount
dll- "$qm$\FreeImage" [_FreeImage_GetFIFDescription@4]$FreeImage_GetFIFDescription fif
dll- "$qm$\FreeImage" [_FreeImage_GetFIFExtensionList@4]$FreeImage_GetFIFExtensionList fif
dll- "$qm$\FreeImage" [_FreeImage_GetFIFFromFilename@4]#FreeImage_GetFIFFromFilename $filename
dll- "$qm$\FreeImage" [_FreeImage_GetFIFFromFilenameU@4]#FreeImage_GetFIFFromFilenameU @*filename
dll- "$qm$\FreeImage" [_FreeImage_GetFIFFromFormat@4]#FreeImage_GetFIFFromFormat $format
dll- "$qm$\FreeImage" [_FreeImage_GetFIFFromMime@4]#FreeImage_GetFIFFromMime $mime
dll- "$qm$\FreeImage" [_FreeImage_GetFIFMimeType@4]$FreeImage_GetFIFMimeType fif
dll- "$qm$\FreeImage" [_FreeImage_GetFIFRegExpr@4]$FreeImage_GetFIFRegExpr fif
dll- "$qm$\FreeImage" [_FreeImage_GetFileType@8]#FreeImage_GetFileType $filename size
dll- "$qm$\FreeImage" [_FreeImage_GetFileTypeFromHandle@12]#FreeImage_GetFileTypeFromHandle FreeImageIO*io !*handle size
dll- "$qm$\FreeImage" [_FreeImage_GetFileTypeFromMemory@8]#FreeImage_GetFileTypeFromMemory FIMEMORY*stream size
dll- "$qm$\FreeImage" [_FreeImage_GetFileTypeU@8]#FreeImage_GetFileTypeU @*filename size
dll- "$qm$\FreeImage" [_FreeImage_GetFormatFromFIF@4]$FreeImage_GetFormatFromFIF fif
dll- "$qm$\FreeImage" [_FreeImage_GetGreenMask@4]#FreeImage_GetGreenMask FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetHeight@4]#FreeImage_GetHeight FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetHistogram@12]#FreeImage_GetHistogram FIBITMAP*dib *histo channel
dll- "$qm$\FreeImage" [_FreeImage_GetICCProfile@4]FIICCPROFILE*FreeImage_GetICCProfile FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetImageType@4]#FreeImage_GetImageType FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetInfo@4]BITMAPINFO*FreeImage_GetInfo FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetInfoHeader@4]BITMAPINFOHEADER*FreeImage_GetInfoHeader FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetLine@4]#FreeImage_GetLine FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetLockedPageNumbers@12]#FreeImage_GetLockedPageNumbers FIMULTIBITMAP*bitmap *pages *count
dll- "$qm$\FreeImage" [_FreeImage_GetMetadata@16]#FreeImage_GetMetadata model FIBITMAP*dib $key FITAG**tag
dll- "$qm$\FreeImage" [_FreeImage_GetMetadataCount@8]#FreeImage_GetMetadataCount model FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetPageCount@4]#FreeImage_GetPageCount FIMULTIBITMAP*bitmap
dll- "$qm$\FreeImage" [_FreeImage_GetPalette@4]RGBQUAD*FreeImage_GetPalette FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetPitch@4]#FreeImage_GetPitch FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetPixelColor@16]#FreeImage_GetPixelColor FIBITMAP*dib x'a y'b RGBQUAD*value
dll- "$qm$\FreeImage" [_FreeImage_GetPixelIndex@16]#FreeImage_GetPixelIndex FIBITMAP*dib x'a y'b !*value
dll- "$qm$\FreeImage" [_FreeImage_GetRedMask@4]#FreeImage_GetRedMask FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetScanLine@8]!*FreeImage_GetScanLine FIBITMAP*dib scanline
dll- "$qm$\FreeImage" [_FreeImage_GetTagCount@4]#FreeImage_GetTagCount FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_GetTagDescription@4]$FreeImage_GetTagDescription FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_GetTagID@4]@FreeImage_GetTagID FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_GetTagKey@4]$FreeImage_GetTagKey FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_GetTagLength@4]#FreeImage_GetTagLength FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_GetTagType@4]#FreeImage_GetTagType FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_GetTagValue@4]!*FreeImage_GetTagValue FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_GetThumbnail@4]FIBITMAP*FreeImage_GetThumbnail FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetTransparencyCount@4]#FreeImage_GetTransparencyCount FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetTransparencyTable@4]!*FreeImage_GetTransparencyTable FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetTransparentIndex@4]#FreeImage_GetTransparentIndex FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_GetVersion@0]$FreeImage_GetVersion
dll- "$qm$\FreeImage" [_FreeImage_GetWidth@4]#FreeImage_GetWidth FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_HasBackgroundColor@4]#FreeImage_HasBackgroundColor FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_HasPixels@4]#FreeImage_HasPixels FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_Initialise@4]FreeImage_Initialise load_local_plugins_only
dll- "$qm$\FreeImage" [_FreeImage_InsertPage@12]FreeImage_InsertPage FIMULTIBITMAP*bitmap page FIBITMAP*data
dll- "$qm$\FreeImage" [_FreeImage_Invert@4]#FreeImage_Invert FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_IsLittleEndian@0]#FreeImage_IsLittleEndian
dll- "$qm$\FreeImage" [_FreeImage_IsPluginEnabled@4]#FreeImage_IsPluginEnabled fif
dll- "$qm$\FreeImage" [_FreeImage_IsTransparent@4]#FreeImage_IsTransparent FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_JPEGCrop@24]#FreeImage_JPEGCrop $src_file $dst_file left top right bottom
dll- "$qm$\FreeImage" [_FreeImage_JPEGCropU@24]#FreeImage_JPEGCropU @*src_file @*dst_file left top right bottom
dll- "$qm$\FreeImage" [_FreeImage_JPEGTransform@16]#FreeImage_JPEGTransform $src_file $dst_file operation perfect
dll- "$qm$\FreeImage" [_FreeImage_JPEGTransformU@16]#FreeImage_JPEGTransformU @*src_file @*dst_file operation perfect
dll- "$qm$\FreeImage" [_FreeImage_Load@12]FIBITMAP*FreeImage_Load fif $filename flags
dll- "$qm$\FreeImage" [_FreeImage_LoadFromHandle@16]FIBITMAP*FreeImage_LoadFromHandle fif FreeImageIO*io !*handle flags
dll- "$qm$\FreeImage" [_FreeImage_LoadFromMemory@12]FIBITMAP*FreeImage_LoadFromMemory fif FIMEMORY*stream flags
dll- "$qm$\FreeImage" [_FreeImage_LoadMultiBitmapFromMemory@12]FIMULTIBITMAP*FreeImage_LoadMultiBitmapFromMemory fif FIMEMORY*stream flags
dll- "$qm$\FreeImage" [_FreeImage_LoadU@12]FIBITMAP*FreeImage_LoadU fif @*filename flags
dll- "$qm$\FreeImage" [_FreeImage_LockPage@8]FIBITMAP*FreeImage_LockPage FIMULTIBITMAP*bitmap page
dll- "$qm$\FreeImage" [_FreeImage_LookupSVGColor@16]#FreeImage_LookupSVGColor $szColor !*nRed !*nGreen !*nBlue
dll- "$qm$\FreeImage" [_FreeImage_LookupX11Color@16]#FreeImage_LookupX11Color $szColor !*nRed !*nGreen !*nBlue
dll- "$qm$\FreeImage" [_FreeImage_MakeThumbnail@12]FIBITMAP*FreeImage_MakeThumbnail FIBITMAP*dib max_pixel_size convert
dll- "$qm$\FreeImage" [_FreeImage_MovePage@12]#FreeImage_MovePage FIMULTIBITMAP*bitmap target source
dll- "$qm$\FreeImage" [_FreeImage_MultigridPoissonSolver@8]FIBITMAP*FreeImage_MultigridPoissonSolver FIBITMAP*Laplacian ncycle
dll- "$qm$\FreeImage" [_FreeImage_OpenMemory@8]FIMEMORY*FreeImage_OpenMemory !*data size_in_bytes
dll- "$qm$\FreeImage" [_FreeImage_OpenMultiBitmap@24]FIMULTIBITMAP*FreeImage_OpenMultiBitmap fif $filename create_new read_only keep_cache_in_memory flags
dll- "$qm$\FreeImage" [_FreeImage_OpenMultiBitmapFromHandle@16]FIMULTIBITMAP*FreeImage_OpenMultiBitmapFromHandle fif FreeImageIO*io !*handle flags
type FreeImage_OutputMessageFunction = #
 ;;function fif $msg
type FreeImage_OutputMessageFunctionStdCall = #
 ;;function fif $msg
dll- "$qm$\FreeImage" [FreeImage_OutputMessageProc]FreeImage_OutputMessageProc fif $fmt ...
dll- "$qm$\FreeImage" [_FreeImage_Paste@20]#FreeImage_Paste FIBITMAP*dst FIBITMAP*src left top alpha
dll- "$qm$\FreeImage" [_FreeImage_PreMultiplyWithAlpha@4]#FreeImage_PreMultiplyWithAlpha FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_ReadMemory@16]#FreeImage_ReadMemory !*buffer size'a count'b FIMEMORY*stream
dll- "$qm$\FreeImage" [_FreeImage_RegisterExternalPlugin@20]#FreeImage_RegisterExternalPlugin $path $format $description $extension $regexpr
dll- "$qm$\FreeImage" [_FreeImage_RegisterLocalPlugin@20]#FreeImage_RegisterLocalPlugin proc_address $format $description $extension $regexpr
 ;;proc_address: function Plugin*plugin format_id
dll- "$qm$\FreeImage" [_FreeImage_Rescale@16]FIBITMAP*FreeImage_Rescale FIBITMAP*dib dst_width dst_height filter
dll- "$qm$\FreeImage" [_FreeImage_Rotate@16]FIBITMAP*FreeImage_Rotate FIBITMAP*dib ^angle !*bkcolor
dll- "$qm$\FreeImage" [_FreeImage_RotateClassic@12]FIBITMAP*FreeImage_RotateClassic FIBITMAP*dib ^angle
dll- "$qm$\FreeImage" [_FreeImage_RotateEx@48]FIBITMAP*FreeImage_RotateEx FIBITMAP*dib ^angle ^x_shift ^y_shift ^x_origin ^y_origin use_mask
dll- "$qm$\FreeImage" [_FreeImage_Save@16]#FreeImage_Save fif FIBITMAP*dib $filename flags
dll- "$qm$\FreeImage" [_FreeImage_SaveMultiBitmapToHandle@20]#FreeImage_SaveMultiBitmapToHandle fif FIMULTIBITMAP*bitmap FreeImageIO*io !*handle flags
dll- "$qm$\FreeImage" [_FreeImage_SaveMultiBitmapToMemory@16]#FreeImage_SaveMultiBitmapToMemory fif FIMULTIBITMAP*bitmap FIMEMORY*stream flags
dll- "$qm$\FreeImage" [_FreeImage_SaveToHandle@20]#FreeImage_SaveToHandle fif FIBITMAP*dib FreeImageIO*io !*handle flags
dll- "$qm$\FreeImage" [_FreeImage_SaveToMemory@16]#FreeImage_SaveToMemory fif FIBITMAP*dib FIMEMORY*stream flags
dll- "$qm$\FreeImage" [_FreeImage_SaveU@16]#FreeImage_SaveU fif FIBITMAP*dib @*filename flags
dll- "$qm$\FreeImage" [_FreeImage_SeekMemory@12]#FreeImage_SeekMemory FIMEMORY*stream offset origin
dll- "$qm$\FreeImage" [_FreeImage_SetBackgroundColor@8]#FreeImage_SetBackgroundColor FIBITMAP*dib RGBQUAD*bkcolor
dll- "$qm$\FreeImage" [_FreeImage_SetChannel@12]#FreeImage_SetChannel FIBITMAP*dst FIBITMAP*src channel
dll- "$qm$\FreeImage" [_FreeImage_SetComplexChannel@12]#FreeImage_SetComplexChannel FIBITMAP*dst FIBITMAP*src channel
dll- "$qm$\FreeImage" [_FreeImage_SetDotsPerMeterX@8]FreeImage_SetDotsPerMeterX FIBITMAP*dib res'a
dll- "$qm$\FreeImage" [_FreeImage_SetDotsPerMeterY@8]FreeImage_SetDotsPerMeterY FIBITMAP*dib res'a
dll- "$qm$\FreeImage" [_FreeImage_SetMetadata@16]#FreeImage_SetMetadata model FIBITMAP*dib $key FITAG*tag
dll- "$qm$\FreeImage" [_FreeImage_SetOutputMessage@4]FreeImage_SetOutputMessage omf
 ;;omf: function fif $msg
dll- "$qm$\FreeImage" [_FreeImage_SetOutputMessageStdCall@4]FreeImage_SetOutputMessageStdCall omf
 ;;omf: function fif $msg
dll- "$qm$\FreeImage" [_FreeImage_SetPixelColor@16]#FreeImage_SetPixelColor FIBITMAP*dib x'a y'b RGBQUAD*value
dll- "$qm$\FreeImage" [_FreeImage_SetPixelIndex@16]#FreeImage_SetPixelIndex FIBITMAP*dib x'a y'b !*value
dll- "$qm$\FreeImage" [_FreeImage_SetPluginEnabled@8]#FreeImage_SetPluginEnabled fif enable
dll- "$qm$\FreeImage" [_FreeImage_SetTagCount@8]#FreeImage_SetTagCount FITAG*tag count
dll- "$qm$\FreeImage" [_FreeImage_SetTagDescription@8]#FreeImage_SetTagDescription FITAG*tag $description
dll- "$qm$\FreeImage" [_FreeImage_SetTagID@8]#FreeImage_SetTagID FITAG*tag @id
dll- "$qm$\FreeImage" [_FreeImage_SetTagKey@8]#FreeImage_SetTagKey FITAG*tag $key
dll- "$qm$\FreeImage" [_FreeImage_SetTagLength@8]#FreeImage_SetTagLength FITAG*tag length
dll- "$qm$\FreeImage" [_FreeImage_SetTagType@8]#FreeImage_SetTagType FITAG*tag type
dll- "$qm$\FreeImage" [_FreeImage_SetTagValue@8]#FreeImage_SetTagValue FITAG*tag !*value
dll- "$qm$\FreeImage" [_FreeImage_SetThumbnail@8]#FreeImage_SetThumbnail FIBITMAP*dib FIBITMAP*thumbnail
dll- "$qm$\FreeImage" [_FreeImage_SetTransparencyTable@12]FreeImage_SetTransparencyTable FIBITMAP*dib !*table count
dll- "$qm$\FreeImage" [_FreeImage_SetTransparent@8]FreeImage_SetTransparent FIBITMAP*dib enabled
dll- "$qm$\FreeImage" [_FreeImage_SetTransparentIndex@8]FreeImage_SetTransparentIndex FIBITMAP*dib index
dll- "$qm$\FreeImage" [_FreeImage_SwapColors@16]#FreeImage_SwapColors FIBITMAP*dib RGBQUAD*color_a RGBQUAD*color_b ignore_alpha
dll- "$qm$\FreeImage" [_FreeImage_SwapPaletteIndices@12]#FreeImage_SwapPaletteIndices FIBITMAP*dib !*index_a !*index_b
dll- "$qm$\FreeImage" [_FreeImage_TagToString@12]$FreeImage_TagToString model FITAG*tag $Make
dll- "$qm$\FreeImage" [_FreeImage_TellMemory@4]#FreeImage_TellMemory FIMEMORY*stream
dll- "$qm$\FreeImage" [_FreeImage_Threshold@8]FIBITMAP*FreeImage_Threshold FIBITMAP*dib !T
dll- "$qm$\FreeImage" [_FreeImage_TmoDrago03@20]FIBITMAP*FreeImage_TmoDrago03 FIBITMAP*src ^gamma ^exposure
dll- "$qm$\FreeImage" [_FreeImage_TmoFattal02@20]FIBITMAP*FreeImage_TmoFattal02 FIBITMAP*src ^color_saturation ^attenuation
dll- "$qm$\FreeImage" [_FreeImage_TmoReinhard05@20]FIBITMAP*FreeImage_TmoReinhard05 FIBITMAP*src ^intensity ^contrast
dll- "$qm$\FreeImage" [_FreeImage_TmoReinhard05Ex@36]FIBITMAP*FreeImage_TmoReinhard05Ex FIBITMAP*src ^intensity ^contrast ^adaptation ^color_correction
dll- "$qm$\FreeImage" [_FreeImage_ToneMapping@24]FIBITMAP*FreeImage_ToneMapping FIBITMAP*dib tmo ^first_param ^second_param
dll- "$qm$\FreeImage" [_FreeImage_Unload@4]FreeImage_Unload FIBITMAP*dib
dll- "$qm$\FreeImage" [_FreeImage_UnlockPage@12]FreeImage_UnlockPage FIMULTIBITMAP*bitmap FIBITMAP*data changed
dll- "$qm$\FreeImage" [_FreeImage_WriteMemory@16]#FreeImage_WriteMemory !*buffer size'a count'b FIMEMORY*stream
dll- "$qm$\FreeImage" [_FreeImage_ZLibCRC32@12]#FreeImage_ZLibCRC32 crc !*source source_size
dll- "$qm$\FreeImage" [_FreeImage_ZLibCompress@16]#FreeImage_ZLibCompress !*target target_size !*source source_size
dll- "$qm$\FreeImage" [_FreeImage_ZLibGUnzip@16]#FreeImage_ZLibGUnzip !*target target_size !*source source_size
dll- "$qm$\FreeImage" [_FreeImage_ZLibGZip@16]#FreeImage_ZLibGZip !*target target_size !*source source_size
dll- "$qm$\FreeImage" [_FreeImage_ZLibUncompress@16]#FreeImage_ZLibUncompress !*target target_size !*source source_size
def GIF_DEFAULT 0
def GIF_LOAD256 1
def GIF_PLAYBACK 2
def HDR_DEFAULT 0
def ICO_DEFAULT 0
def ICO_MAKEALPHA 1
def IFF_DEFAULT 0
def J2K_DEFAULT 0
def JP2_DEFAULT 0
def JPEG_ACCURATE 0x0002
def JPEG_BASELINE 0x40000
def JPEG_CMYK 0x0004
def JPEG_DEFAULT 0
def JPEG_EXIFROTATE 0x0008
def JPEG_FAST 0x0001
def JPEG_GREYSCALE 0x0010
def JPEG_OPTIMIZE 0x20000
def JPEG_PROGRESSIVE 0x2000
def JPEG_QUALITYAVERAGE 0x0400
def JPEG_QUALITYBAD 0x0800
def JPEG_QUALITYGOOD 0x0100
def JPEG_QUALITYNORMAL 0x0200
def JPEG_QUALITYSUPERB 0x80
def JPEG_SUBSAMPLING_411 0x1000
def JPEG_SUBSAMPLING_420 0x4000
def JPEG_SUBSAMPLING_422 0x8000
def JPEG_SUBSAMPLING_444 0x10000
def KOALA_DEFAULT 0
def LBM_DEFAULT 0
def MNG_DEFAULT 0
def PCD_BASE 1
def PCD_BASEDIV16 3
def PCD_BASEDIV4 2
def PCD_DEFAULT 0
def PCX_DEFAULT 0
def PFM_DEFAULT 0
def PICT_DEFAULT 0
def PLUGINS
def PNG_DEFAULT 0
def PNG_IGNOREGAMMA 1
def PNG_INTERLACED 0x0200
def PNG_Z_BEST_COMPRESSION 0x0009
def PNG_Z_BEST_SPEED 0x0001
def PNG_Z_DEFAULT_COMPRESSION 0x0006
def PNG_Z_NO_COMPRESSION 0x0100
def PNM_DEFAULT 0
def PNM_SAVE_ASCII 1
def PNM_SAVE_RAW 0
def PSD_CMYK 1
def PSD_DEFAULT 0
def PSD_LAB 2
type Plugin format_proc description_proc extension_proc regexpr_proc open_proc close_proc pagecount_proc pagecapability_proc load_proc save_proc validate_proc mime_proc supports_export_bpp_proc supports_export_type_proc supports_icc_profiles_proc supports_no_pixels_proc
def RAS_DEFAULT 0
def RAW_DEFAULT 0
def RAW_DISPLAY 2
def RAW_HALFSIZE 4
def RAW_PREVIEW 1
def SGI_DEFAULT 0
def TARGA_DEFAULT 0
def TARGA_LOAD_RGB888 1
def TARGA_SAVE_RLE 2
def TIFF_ADOBE_DEFLATE 0x0400
def TIFF_CCITTFAX3 0x1000
def TIFF_CCITTFAX4 0x2000
def TIFF_CMYK 0x0001
def TIFF_DEFAULT 0
def TIFF_DEFLATE 0x0200
def TIFF_JPEG 0x8000
def TIFF_LOGLUV 0x10000
def TIFF_LZW 0x4000
def TIFF_NONE 0x0800
def TIFF_PACKBITS 0x0100
def WBMP_DEFAULT 0
def XBM_DEFAULT 0
def XPM_DEFAULT 0
