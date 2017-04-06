
str f="$qm$\bmp_test\TVimagelist.bmp" ;;4-bit
 str f="$qm$\bmp_test\WinCal_IDB_TOOLBARBITMAP_8.bmp" ;;8-bit
 str f="$qm$\bmp_test\shell32Vista_IDB_TB_EXT_DEF_16.bmp" ;;24-bit
 str f="$qm$\bmp_test\shell32Vista_IDB_TB_IE_DEF_16.bmp" ;;32-bit no mask
 str f="$qm$\bmp_test\Maxthon_9101.bmp" ;;32-bit with white mask
 str f="$qm$\bmp_test\AMBASFnc_BBRETRY.bmp" ;;4-bit 18 px
 str f="$qm$\bmp_test\256.bmp" ;;32-bit 256 px

out
out __BitmapToIcons(f "$temp$" -1 1)
 out __BitmapToIcons(f "$qm$\bmp_icons" 0xff00ff)
