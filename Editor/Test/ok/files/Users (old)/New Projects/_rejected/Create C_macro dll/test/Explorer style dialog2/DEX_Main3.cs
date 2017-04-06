
type DEX_DATA il ;;you can add more members to store various data
DEX_DATA d ;;to access this variable from dialog procedure, use DEX_DATA& d; &d=+DT_GetParam(hDlg)

#if QMVER>=0x2030003
 add bitmap containing all icons
d.il=__ImageListLoad("$qm$\il_qm.bmp") ;;the function loads an imagelist created with the imagelist editor. The function and the editor are unavailable in older QM versions. Instead can be used for example ImageList_LoadImage.
#else
 load multiple files (slower):
str iconlist=
 $qm$\close.ico
 $qm$\controls.ico
 $qm$\copy.ico
 $qm$\cut.ico
 $qm$\del.ico
 $qm$\dialog.ico
 $qm$\email.ico
 $qm$\favorites.ico
 $qm$\files.ico
 $qm$\find.ico
d.il=ImageList_Create(16 16 ILC_MASK|ILC_COLOR32 0 10)
for(_i 0 10) _s.getl(iconlist -_i); int hi=GetIcon(_s); ImageList_ReplaceIcon(d.il -1 hi); DestroyIcon hi

  or add bitmap containing all icons
 d.il=ImageList_LoadImage(0 _s.expandpath("$my qm$\for_imagelist.bmp") 16 0 0xFF00FF IMAGE_BITMAP LR_LOADFROMFILE)
#endif

if(!ShowDialog("DEX_Dialog3" &DEX_Dialog3 0 0 0 0 0 &d)) ret

ImageList_Destroy(d.il)
