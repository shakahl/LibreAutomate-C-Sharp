#include <windows.h>
#undef Animate_Close
__declspec(dllexport) int Animate_Close(hwnd) { return SendMessage((HWND)hwnd,(0x0400+100),0,0); }
#undef Animate_IsPlaying
__declspec(dllexport) int Animate_IsPlaying(hwnd) { return SendMessage((HWND)hwnd,(0x0400+104),0,0); }
#undef Animate_Open
__declspec(dllexport) int Animate_Open(hwnd, szName) { return SendMessage((HWND)hwnd,(0x0400+100),0,szName); }
#undef Animate_OpenEx
__declspec(dllexport) int Animate_OpenEx(hwnd, hInst, szName) { return SendMessage((HWND)hwnd,(0x0400+100),hInst,szName); }
#undef Animate_Play
__declspec(dllexport) int Animate_Play(hwnd, from, to, rep) { return SendMessage((HWND)hwnd,(0x0400+101),rep,(((((from)&0xffff))|((((to)&0xffff)))<<16))); }
#undef Animate_Seek
__declspec(dllexport) int Animate_Seek(hwnd, frame) { return SendMessage((HWND)hwnd,(0x0400+101),1,(((((frame)&0xffff))|((((frame)&0xffff)))<<16))); }
#undef Animate_Stop
__declspec(dllexport) int Animate_Stop(hwnd) { return SendMessage((HWND)hwnd,(0x0400+102),0,0); }
#undef Button_GetIdealSize
__declspec(dllexport) int Button_GetIdealSize(hwnd, psize) { return SendMessage((HWND)hwnd,(0x1600+0x0001),0,psize); }
#undef Button_GetImageList
__declspec(dllexport) int Button_GetImageList(hwnd, pbuttonImagelist) { return SendMessage((HWND)hwnd,(0x1600+0x0003),0,pbuttonImagelist); }
#undef Button_GetTextMargin
__declspec(dllexport) int Button_GetTextMargin(hwnd, pmargin) { return SendMessage((HWND)hwnd,(0x1600+0x0005),0,pmargin); }
#undef Button_SetImageList
__declspec(dllexport) int Button_SetImageList(hwnd, pbuttonImagelist) { return SendMessage((HWND)hwnd,(0x1600+0x0002),0,pbuttonImagelist); }
#undef Button_SetTextMargin
__declspec(dllexport) int Button_SetTextMargin(hwnd, pmargin) { return SendMessage((HWND)hwnd,(0x1600+0x0004),0,pmargin); }
#undef ComboBox_GetCueBannerText
__declspec(dllexport) int ComboBox_GetCueBannerText(hwnd, lpwText, cchText) { return SendMessage((HWND)hwnd,(0x1700+4),lpwText,cchText); }
#undef ComboBox_GetMinVisible
__declspec(dllexport) int ComboBox_GetMinVisible(hwnd) { return SendMessage((HWND)hwnd,(0x1700+2),0,0); }
#undef ComboBox_SetCueBannerText
__declspec(dllexport) int ComboBox_SetCueBannerText(hwnd, lpcwText) { return SendMessage((HWND)hwnd,(0x1700+3),0,lpcwText); }
#undef ComboBox_SetMinVisible
__declspec(dllexport) int ComboBox_SetMinVisible(hwnd, iMinVisible) { return SendMessage((HWND)hwnd,(0x1700+1),iMinVisible,0); }
#undef CommDlg_OpenSave_GetFilePath
__declspec(dllexport) int CommDlg_OpenSave_GetFilePath(_hdlg, _psz, _cbmax) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0001),_cbmax,_psz); }
#undef CommDlg_OpenSave_GetFilePathW
__declspec(dllexport) int CommDlg_OpenSave_GetFilePathW(_hdlg, _psz, _cbmax) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0001),_cbmax,_psz); }
#undef CommDlg_OpenSave_GetFolderIDList
__declspec(dllexport) int CommDlg_OpenSave_GetFolderIDList(_hdlg, _pidl, _cbmax) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0003),_cbmax,_pidl); }
#undef CommDlg_OpenSave_GetFolderPath
__declspec(dllexport) int CommDlg_OpenSave_GetFolderPath(_hdlg, _psz, _cbmax) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0002),_cbmax,_psz); }
#undef CommDlg_OpenSave_GetFolderPathW
__declspec(dllexport) int CommDlg_OpenSave_GetFolderPathW(_hdlg, _psz, _cbmax) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0002),_cbmax,_psz); }
#undef CommDlg_OpenSave_GetSpec
__declspec(dllexport) int CommDlg_OpenSave_GetSpec(_hdlg, _psz, _cbmax) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0000),_cbmax,_psz); }
#undef CommDlg_OpenSave_GetSpecW
__declspec(dllexport) int CommDlg_OpenSave_GetSpecW(_hdlg, _psz, _cbmax) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0000),_cbmax,_psz); }
#undef CommDlg_OpenSave_HideControl
__declspec(dllexport) int CommDlg_OpenSave_HideControl(_hdlg, _id) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0005),_id,0); }
#undef CommDlg_OpenSave_SetControlText
__declspec(dllexport) int CommDlg_OpenSave_SetControlText(_hdlg, _id, _text) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0004),_id,_text); }
#undef CommDlg_OpenSave_SetDefExt
__declspec(dllexport) int CommDlg_OpenSave_SetDefExt(_hdlg, _pszext) { return SendMessage((HWND)_hdlg,((0x0400+100)+0x0006),0,_pszext); }
#undef DateTime_GetMonthCal
__declspec(dllexport) int DateTime_GetMonthCal(hdp) { return SendMessage((HWND)hdp,(0x1000+8),0,0); }
#undef DateTime_GetMonthCalColor
__declspec(dllexport) int DateTime_GetMonthCalColor(hdp, iColor) { return SendMessage((HWND)hdp,(0x1000+7),iColor,0); }
#undef DateTime_GetMonthCalFont
__declspec(dllexport) int DateTime_GetMonthCalFont(hdp) { return SendMessage((HWND)hdp,(0x1000+10),0,0); }
#undef DateTime_GetRange
__declspec(dllexport) int DateTime_GetRange(hdp, rgst) { return SendMessage((HWND)hdp,(0x1000+3),0,rgst); }
#undef DateTime_GetSystemtime
__declspec(dllexport) int DateTime_GetSystemtime(hdp, pst) { return SendMessage((HWND)hdp,(0x1000+1),0,pst); }
#undef DateTime_SetFormat
__declspec(dllexport) int DateTime_SetFormat(hdp, sz) { return SendMessage((HWND)hdp,(0x1000+5),0,sz); }
#undef DateTime_SetMonthCalColor
__declspec(dllexport) int DateTime_SetMonthCalColor(hdp, iColor, clr) { return SendMessage((HWND)hdp,(0x1000+6),iColor,clr); }
#undef DateTime_SetMonthCalFont
__declspec(dllexport) int DateTime_SetMonthCalFont(hdp, hfont, fRedraw) { return SendMessage((HWND)hdp,(0x1000+9),hfont,fRedraw); }
#undef DateTime_SetRange
__declspec(dllexport) int DateTime_SetRange(hdp, gd, rgst) { return SendMessage((HWND)hdp,(0x1000+4),gd,rgst); }
#undef DateTime_SetSystemtime
__declspec(dllexport) int DateTime_SetSystemtime(hdp, gd, pst) { return SendMessage((HWND)hdp,(0x1000+2),gd,pst); }
#undef Edit_GetCueBannerText
__declspec(dllexport) int Edit_GetCueBannerText(hwnd, lpwText, cchText) { return SendMessage((HWND)hwnd,(0x1500+2),lpwText,cchText); }
#undef Edit_HideBalloonTip
__declspec(dllexport) int Edit_HideBalloonTip(hwnd) { return SendMessage((HWND)hwnd,(0x1500+4),0,0); }
#undef Edit_SetCueBannerText
__declspec(dllexport) int Edit_SetCueBannerText(hwnd, lpcwText) { return SendMessage((HWND)hwnd,(0x1500+1),0,lpcwText); }
#undef Edit_SetCueBannerTextFocused
__declspec(dllexport) int Edit_SetCueBannerTextFocused(hwnd, lpcwText, fDrawFocused) { return SendMessage((HWND)hwnd,(0x1500+1),fDrawFocused,lpcwText); }
#undef Edit_ShowBalloonTip
__declspec(dllexport) int Edit_ShowBalloonTip(hwnd, peditballoontip) { return SendMessage((HWND)hwnd,(0x1500+3),0,peditballoontip); }
#undef Header_ClearAllFilters
__declspec(dllexport) int Header_ClearAllFilters(hwnd) { return SendMessage((HWND)hwnd,(0x1200+24),-1,0); }
#undef Header_ClearFilter
__declspec(dllexport) int Header_ClearFilter(hwnd, i) { return SendMessage((HWND)hwnd,(0x1200+24),i,0); }
#undef Header_CreateDragImage
__declspec(dllexport) int Header_CreateDragImage(hwnd, i) { return SendMessage((HWND)hwnd,(0x1200+16),i,0); }
#undef Header_DeleteItem
__declspec(dllexport) int Header_DeleteItem(hwndHD, i) { return SendMessage((HWND)hwndHD,(0x1200+2),i,0); }
#undef Header_EditFilter
__declspec(dllexport) int Header_EditFilter(hwnd, i, fDiscardChanges) { return SendMessage((HWND)hwnd,(0x1200+23),i,((((((fDiscardChanges)&0xffff))|((((0)&0xffff)))<<16)))); }
#undef Header_GetBitmapMargin
__declspec(dllexport) int Header_GetBitmapMargin(hwnd) { return SendMessage((HWND)hwnd,(0x1200+21),0,0); }
#undef Header_GetImageList
__declspec(dllexport) int Header_GetImageList(hwnd) { return SendMessage((HWND)hwnd,(0x1200+9),0,0); }
#undef Header_GetItem
__declspec(dllexport) int Header_GetItem(hwndHD, i, phdi) { return SendMessage((HWND)hwndHD,(0x1200+3),i,phdi); }
#undef Header_GetItemCount
__declspec(dllexport) int Header_GetItemCount(hwndHD) { return SendMessage((HWND)hwndHD,(0x1200+0),0,0); }
#undef Header_GetItemRect
__declspec(dllexport) int Header_GetItemRect(hwnd, iItem, lprc) { return SendMessage((HWND)hwnd,(0x1200+7),iItem,lprc); }
#undef Header_GetOrderArray
__declspec(dllexport) int Header_GetOrderArray(hwnd, iCount, lpi) { return SendMessage((HWND)hwnd,(0x1200+17),iCount,lpi); }
#undef Header_GetStateImageList
__declspec(dllexport) int Header_GetStateImageList(hwnd) { return SendMessage((HWND)hwnd,(0x1200+9),1,0); }
#undef Header_GetUnicodeFormat
__declspec(dllexport) int Header_GetUnicodeFormat(hwnd) { return SendMessage((HWND)hwnd,(0x2000+6),0,0); }
#undef Header_InsertItem
__declspec(dllexport) int Header_InsertItem(hwndHD, i, phdi) { return SendMessage((HWND)hwndHD,(0x1200+1),i,(phdi)); }
#undef Header_Layout
__declspec(dllexport) int Header_Layout(hwndHD, playout) { return SendMessage((HWND)hwndHD,(0x1200+5),0,playout); }
#undef Header_OrderToIndex
__declspec(dllexport) int Header_OrderToIndex(hwnd, i) { return SendMessage((HWND)hwnd,(0x1200+15),i,0); }
#undef Header_SetBitmapMargin
__declspec(dllexport) int Header_SetBitmapMargin(hwnd, iWidth) { return SendMessage((HWND)hwnd,(0x1200+20),iWidth,0); }
#undef Header_SetFilterChangeTimeout
__declspec(dllexport) int Header_SetFilterChangeTimeout(hwnd, i) { return SendMessage((HWND)hwnd,(0x1200+22),0,i); }
#undef Header_SetHotDivider
__declspec(dllexport) int Header_SetHotDivider(hwnd, fPos, dw) { return SendMessage((HWND)hwnd,(0x1200+19),fPos,dw); }
#undef Header_SetImageList
__declspec(dllexport) int Header_SetImageList(hwnd, himl) { return SendMessage((HWND)hwnd,(0x1200+8),0,himl); }
#undef Header_SetItem
__declspec(dllexport) int Header_SetItem(hwndHD, i, phdi) { return SendMessage((HWND)hwndHD,(0x1200+4),i,(phdi)); }
#undef Header_SetOrderArray
__declspec(dllexport) int Header_SetOrderArray(hwnd, iCount, lpi) { return SendMessage((HWND)hwnd,(0x1200+18),iCount,lpi); }
#undef Header_SetStateImageList
__declspec(dllexport) int Header_SetStateImageList(hwnd, himl) { return SendMessage((HWND)hwnd,(0x1200+8),1,himl); }
#undef Header_SetUnicodeFormat
__declspec(dllexport) int Header_SetUnicodeFormat(hwnd, fUnicode) { return SendMessage((HWND)hwnd,(0x2000+5),fUnicode,0); }
#undef ListView_ApproximateViewRect
__declspec(dllexport) int ListView_ApproximateViewRect(hwnd, iWidth, iHeight, iCount) { return SendMessage((HWND)hwnd,(0x1000+64),iCount,((((((iWidth)&0xffff))|((((iHeight)&0xffff)))<<16)))); }
#undef ListView_Arrange
__declspec(dllexport) int ListView_Arrange(hwndLV, code) { return SendMessage((HWND)hwndLV,(0x1000+22),code,0); }
#undef ListView_CancelEditLabel
__declspec(dllexport) int ListView_CancelEditLabel(hwnd) { return SendMessage((HWND)hwnd,(0x1000+179),0,0); }
#undef ListView_CreateDragImage
__declspec(dllexport) int ListView_CreateDragImage(hwnd, i, lpptUpLeft) { return SendMessage((HWND)hwnd,(0x1000+33),i,lpptUpLeft); }
#undef ListView_DeleteAllItems
__declspec(dllexport) int ListView_DeleteAllItems(hwnd) { return SendMessage((HWND)hwnd,(0x1000+9),0,0); }
#undef ListView_DeleteColumn
__declspec(dllexport) int ListView_DeleteColumn(hwnd, iCol) { return SendMessage((HWND)hwnd,(0x1000+28),iCol,0); }
#undef ListView_DeleteItem
__declspec(dllexport) int ListView_DeleteItem(hwnd, i) { return SendMessage((HWND)hwnd,(0x1000+8),i,0); }
#undef ListView_EditLabel
__declspec(dllexport) int ListView_EditLabel(hwndLV, i) { return SendMessage((HWND)hwndLV,(0x1000+23),i,0); }
#undef ListView_EnableGroupView
__declspec(dllexport) int ListView_EnableGroupView(hwnd, fEnable) { return SendMessage((HWND)hwnd,(0x1000+157),fEnable,0); }
#undef ListView_EnsureVisible
__declspec(dllexport) int ListView_EnsureVisible(hwndLV, i, fPartialOK) { return SendMessage((HWND)hwndLV,(0x1000+19),i,(((((((fPartialOK))&0xffff))|((((0)&0xffff)))<<16)))); }
#undef ListView_FindItem
__declspec(dllexport) int ListView_FindItem(hwnd, iStart, plvfi) { return SendMessage((HWND)hwnd,(0x1000+13),iStart,(plvfi)); }
#undef ListView_GetBkColor
__declspec(dllexport) int ListView_GetBkColor(hwnd) { return SendMessage((HWND)hwnd,(0x1000+0),0,0); }
#undef ListView_GetBkImage
__declspec(dllexport) int ListView_GetBkImage(hwnd, plvbki) { return SendMessage((HWND)hwnd,(0x1000+69),0,plvbki); }
#undef ListView_GetCallbackMask
__declspec(dllexport) int ListView_GetCallbackMask(hwnd) { return SendMessage((HWND)hwnd,(0x1000+10),0,0); }
#undef ListView_GetColumn
__declspec(dllexport) int ListView_GetColumn(hwnd, iCol, pcol) { return SendMessage((HWND)hwnd,(0x1000+25),iCol,pcol); }
#undef ListView_GetColumnOrderArray
__declspec(dllexport) int ListView_GetColumnOrderArray(hwnd, iCount, pi) { return SendMessage((HWND)hwnd,(0x1000+59),iCount,pi); }
#undef ListView_GetColumnWidth
__declspec(dllexport) int ListView_GetColumnWidth(hwnd, iCol) { return SendMessage((HWND)hwnd,(0x1000+29),iCol,0); }
#undef ListView_GetCountPerPage
__declspec(dllexport) int ListView_GetCountPerPage(hwndLV) { return SendMessage((HWND)hwndLV,(0x1000+40),0,0); }
#undef ListView_GetEditControl
__declspec(dllexport) int ListView_GetEditControl(hwndLV) { return SendMessage((HWND)hwndLV,(0x1000+24),0,0); }
#undef ListView_GetExtendedListViewStyle
__declspec(dllexport) int ListView_GetExtendedListViewStyle(hwndLV) { return SendMessage((HWND)hwndLV,(0x1000+55),0,0); }
#undef ListView_GetFocusedGroup
__declspec(dllexport) int ListView_GetFocusedGroup(hwnd) { return SendMessage((HWND)hwnd,(0x1000+93),0,0); }
#undef ListView_GetGroupCount
__declspec(dllexport) int ListView_GetGroupCount(hwnd) { return SendMessage((HWND)hwnd,(0x1000+152),0,0); }
#undef ListView_GetGroupInfo
__declspec(dllexport) int ListView_GetGroupInfo(hwnd, iGroupId, pgrp) { return SendMessage((HWND)hwnd,(0x1000+149),iGroupId,pgrp); }
#undef ListView_GetGroupInfoByIndex
__declspec(dllexport) int ListView_GetGroupInfoByIndex(hwnd, iIndex, pgrp) { return SendMessage((HWND)hwnd,(0x1000+153),iIndex,pgrp); }
#undef ListView_GetGroupMetrics
__declspec(dllexport) int ListView_GetGroupMetrics(hwnd, pGroupMetrics) { return SendMessage((HWND)hwnd,(0x1000+156),0,pGroupMetrics); }
#undef ListView_GetGroupRect
__declspec(dllexport) int ListView_GetGroupRect(hwnd, iGroupId, type, prc) { return SendMessage((HWND)hwnd,(0x1000+98),iGroupId,((prc)?(((RECT*)prc)->top=(type)),(prc):0)); }
#undef ListView_GetGroupState
__declspec(dllexport) int ListView_GetGroupState(hwnd, dwGroupId, dwMask) { return SendMessage((HWND)hwnd,(0x1000+92),dwGroupId,dwMask); }
#undef ListView_GetHeader
__declspec(dllexport) int ListView_GetHeader(hwnd) { return SendMessage((HWND)hwnd,(0x1000+31),0,0); }
#undef ListView_GetHotCursor
__declspec(dllexport) int ListView_GetHotCursor(hwnd) { return SendMessage((HWND)hwnd,(0x1000+63),0,0); }
#undef ListView_GetHotItem
__declspec(dllexport) int ListView_GetHotItem(hwnd) { return SendMessage((HWND)hwnd,(0x1000+61),0,0); }
#undef ListView_GetHoverTime
__declspec(dllexport) int ListView_GetHoverTime(hwndLV) { return SendMessage((HWND)hwndLV,(0x1000+72),0,0); }
#undef ListView_GetISearchString
__declspec(dllexport) int ListView_GetISearchString(hwndLV, lpsz) { return SendMessage((HWND)hwndLV,(0x1000+52),0,lpsz); }
#undef ListView_GetImageList
__declspec(dllexport) int ListView_GetImageList(hwnd, iImageList) { return SendMessage((HWND)hwnd,(0x1000+2),iImageList,0); }
#undef ListView_GetInsertMark
__declspec(dllexport) int ListView_GetInsertMark(hwnd, lvim) { return SendMessage((HWND)hwnd,(0x1000+167),0,lvim); }
#undef ListView_GetInsertMarkColor
__declspec(dllexport) int ListView_GetInsertMarkColor(hwnd) { return SendMessage((HWND)hwnd,(0x1000+171),0,0); }
#undef ListView_GetInsertMarkRect
__declspec(dllexport) int ListView_GetInsertMarkRect(hwnd, rc) { return SendMessage((HWND)hwnd,(0x1000+169),0,rc); }
#undef ListView_GetItem
__declspec(dllexport) int ListView_GetItem(hwnd, pitem) { return SendMessage((HWND)hwnd,(0x1000+5),0,pitem); }
#undef ListView_GetItemCount
__declspec(dllexport) int ListView_GetItemCount(hwnd) { return SendMessage((HWND)hwnd,(0x1000+4),0,0); }
#undef ListView_GetItemPosition
__declspec(dllexport) int ListView_GetItemPosition(hwndLV, i, ppt) { return SendMessage((HWND)hwndLV,(0x1000+16),i,ppt); }
#undef ListView_GetItemRect
__declspec(dllexport) int ListView_GetItemRect(hwnd, i, prc, code) { return SendMessage((HWND)hwnd,(0x1000+14),i,((prc)?(((RECT*)prc)->left=(code),prc):0)); }
#undef ListView_GetItemSpacing
__declspec(dllexport) int ListView_GetItemSpacing(hwndLV, fSmall) { return SendMessage((HWND)hwndLV,(0x1000+51),fSmall,0); }
#undef ListView_GetItemState
__declspec(dllexport) int ListView_GetItemState(hwndLV, i, mask) { return SendMessage((HWND)hwndLV,(0x1000+44),i,mask); }
#undef ListView_GetNextItem
__declspec(dllexport) int ListView_GetNextItem(hwnd, i, flags) { return SendMessage((HWND)hwnd,(0x1000+12),i,(((((((flags))&0xffff))|((((0)&0xffff)))<<16)))); }
#undef ListView_GetNumberOfWorkAreas
__declspec(dllexport) int ListView_GetNumberOfWorkAreas(hwnd, pnWorkAreas) { return SendMessage((HWND)hwnd,(0x1000+73),0,pnWorkAreas); }
#undef ListView_GetOrigin
__declspec(dllexport) int ListView_GetOrigin(hwndLV, ppt) { return SendMessage((HWND)hwndLV,(0x1000+41),0,ppt); }
#undef ListView_GetOutlineColor
__declspec(dllexport) int ListView_GetOutlineColor(hwnd) { return SendMessage((HWND)hwnd,(0x1000+176),0,0); }
#undef ListView_GetSelectedColumn
__declspec(dllexport) int ListView_GetSelectedColumn(hwnd) { return SendMessage((HWND)hwnd,(0x1000+174),0,0); }
#undef ListView_GetSelectedCount
__declspec(dllexport) int ListView_GetSelectedCount(hwndLV) { return SendMessage((HWND)hwndLV,(0x1000+50),0,0); }
#undef ListView_GetSelectionMark
__declspec(dllexport) int ListView_GetSelectionMark(hwnd) { return SendMessage((HWND)hwnd,(0x1000+66),0,0); }
#undef ListView_GetStringWidth
__declspec(dllexport) int ListView_GetStringWidth(hwndLV, psz) { return SendMessage((HWND)hwndLV,(0x1000+17),0,psz); }
#undef ListView_GetSubItemRect
__declspec(dllexport) int ListView_GetSubItemRect(hwnd, iItem, iSubItem, code, prc) { return SendMessage((HWND)hwnd,(0x1000+56),iItem,((prc)?((((RECT*)prc)->top=(iSubItem)),(((RECT*)prc)->left=(code)),prc):0)); }
#undef ListView_GetTextBkColor
__declspec(dllexport) int ListView_GetTextBkColor(hwnd) { return SendMessage((HWND)hwnd,(0x1000+37),0,0); }
#undef ListView_GetTextColor
__declspec(dllexport) int ListView_GetTextColor(hwnd) { return SendMessage((HWND)hwnd,(0x1000+35),0,0); }
#undef ListView_GetTileInfo
__declspec(dllexport) int ListView_GetTileInfo(hwnd, pti) { return SendMessage((HWND)hwnd,(0x1000+165),0,pti); }
#undef ListView_GetTileViewInfo
__declspec(dllexport) int ListView_GetTileViewInfo(hwnd, ptvi) { return SendMessage((HWND)hwnd,(0x1000+163),0,ptvi); }
#undef ListView_GetToolTips
__declspec(dllexport) int ListView_GetToolTips(hwndLV) { return SendMessage((HWND)hwndLV,(0x1000+78),0,0); }
#undef ListView_GetTopIndex
__declspec(dllexport) int ListView_GetTopIndex(hwndLV) { return SendMessage((HWND)hwndLV,(0x1000+39),0,0); }
#undef ListView_GetUnicodeFormat
__declspec(dllexport) int ListView_GetUnicodeFormat(hwnd) { return SendMessage((HWND)hwnd,(0x2000+6),0,0); }
#undef ListView_GetView
__declspec(dllexport) int ListView_GetView(hwnd) { return SendMessage((HWND)hwnd,(0x1000+143),0,0); }
#undef ListView_GetViewRect
__declspec(dllexport) int ListView_GetViewRect(hwnd, prc) { return SendMessage((HWND)hwnd,(0x1000+34),0,prc); }
#undef ListView_GetWorkAreas
__declspec(dllexport) int ListView_GetWorkAreas(hwnd, nWorkAreas, prc) { return SendMessage((HWND)hwnd,(0x1000+70),nWorkAreas,prc); }
#undef ListView_HasGroup
__declspec(dllexport) int ListView_HasGroup(hwnd, dwGroupId) { return SendMessage((HWND)hwnd,(0x1000+161),dwGroupId,0); }
#undef ListView_HitTest
__declspec(dllexport) int ListView_HitTest(hwndLV, pinfo) { return SendMessage((HWND)hwndLV,(0x1000+18),0,pinfo); }
#undef ListView_HitTestEx
__declspec(dllexport) int ListView_HitTestEx(hwndLV, pinfo) { return SendMessage((HWND)hwndLV,(0x1000+18),-1,pinfo); }
#undef ListView_InsertColumn
__declspec(dllexport) int ListView_InsertColumn(hwnd, iCol, pcol) { return SendMessage((HWND)hwnd,(0x1000+27),iCol,(pcol)); }
#undef ListView_InsertGroup
__declspec(dllexport) int ListView_InsertGroup(hwnd, index, pgrp) { return SendMessage((HWND)hwnd,(0x1000+145),index,pgrp); }
#undef ListView_InsertGroupSorted
__declspec(dllexport) int ListView_InsertGroupSorted(hwnd, structInsert) { return SendMessage((HWND)hwnd,(0x1000+159),structInsert,0); }
#undef ListView_InsertItem
__declspec(dllexport) int ListView_InsertItem(hwnd, pitem) { return SendMessage((HWND)hwnd,(0x1000+7),0,(pitem)); }
#undef ListView_InsertMarkHitTest
__declspec(dllexport) int ListView_InsertMarkHitTest(hwnd, point, lvim) { return SendMessage((HWND)hwnd,(0x1000+168),point,lvim); }
#undef ListView_IsGroupViewEnabled
__declspec(dllexport) int ListView_IsGroupViewEnabled(hwnd) { return SendMessage((HWND)hwnd,(0x1000+175),0,0); }
#undef ListView_IsItemVisible
__declspec(dllexport) int ListView_IsItemVisible(hwnd, index) { return SendMessage((HWND)hwnd,(0x1000+182),index,0); }
#undef ListView_MapIDToIndex
__declspec(dllexport) int ListView_MapIDToIndex(hwnd, id) { return SendMessage((HWND)hwnd,(0x1000+181),id,0); }
#undef ListView_MapIndexToID
__declspec(dllexport) int ListView_MapIndexToID(hwnd, index) { return SendMessage((HWND)hwnd,(0x1000+180),index,0); }
#undef ListView_MoveGroup
__declspec(dllexport) int ListView_MoveGroup(hwnd, iGroupId, toIndex) { return SendMessage((HWND)hwnd,(0x1000+151),iGroupId,toIndex); }
#undef ListView_MoveItemToGroup
__declspec(dllexport) int ListView_MoveItemToGroup(hwnd, idItemFrom, idGroupTo) { return SendMessage((HWND)hwnd,(0x1000+154),idItemFrom,idGroupTo); }
#undef ListView_RedrawItems
__declspec(dllexport) int ListView_RedrawItems(hwndLV, iFirst, iLast) { return SendMessage((HWND)hwndLV,(0x1000+21),iFirst,iLast); }
#undef ListView_RemoveAllGroups
__declspec(dllexport) int ListView_RemoveAllGroups(hwnd) { return SendMessage((HWND)hwnd,(0x1000+160),0,0); }
#undef ListView_RemoveGroup
__declspec(dllexport) int ListView_RemoveGroup(hwnd, iGroupId) { return SendMessage((HWND)hwnd,(0x1000+150),iGroupId,0); }
#undef ListView_Scroll
__declspec(dllexport) int ListView_Scroll(hwndLV, dx, dy) { return SendMessage((HWND)hwndLV,(0x1000+20),dx,dy); }
#undef ListView_SetBkColor
__declspec(dllexport) int ListView_SetBkColor(hwnd, clrBk) { return SendMessage((HWND)hwnd,(0x1000+1),0,clrBk); }
#undef ListView_SetBkImage
__declspec(dllexport) int ListView_SetBkImage(hwnd, plvbki) { return SendMessage((HWND)hwnd,(0x1000+68),0,plvbki); }
#undef ListView_SetCallbackMask
__declspec(dllexport) int ListView_SetCallbackMask(hwnd, mask) { return SendMessage((HWND)hwnd,(0x1000+11),mask,0); }
#undef ListView_SetColumn
__declspec(dllexport) int ListView_SetColumn(hwnd, iCol, pcol) { return SendMessage((HWND)hwnd,(0x1000+26),iCol,(pcol)); }
#undef ListView_SetColumnOrderArray
__declspec(dllexport) int ListView_SetColumnOrderArray(hwnd, iCount, pi) { return SendMessage((HWND)hwnd,(0x1000+58),iCount,pi); }
#undef ListView_SetColumnWidth
__declspec(dllexport) int ListView_SetColumnWidth(hwnd, iCol, cx) { return SendMessage((HWND)hwnd,(0x1000+30),iCol,(((((((cx))&0xffff))|((((0)&0xffff)))<<16)))); }
#undef ListView_SetExtendedListViewStyle
__declspec(dllexport) int ListView_SetExtendedListViewStyle(hwndLV, dw) { return SendMessage((HWND)hwndLV,(0x1000+54),0,dw); }
#undef ListView_SetExtendedListViewStyleEx
__declspec(dllexport) int ListView_SetExtendedListViewStyleEx(hwndLV, dwMask, dw) { return SendMessage((HWND)hwndLV,(0x1000+54),dwMask,dw); }
#undef ListView_SetGroupInfo
__declspec(dllexport) int ListView_SetGroupInfo(hwnd, iGroupId, pgrp) { return SendMessage((HWND)hwnd,(0x1000+147),iGroupId,pgrp); }
#undef ListView_SetGroupMetrics
__declspec(dllexport) int ListView_SetGroupMetrics(hwnd, pGroupMetrics) { return SendMessage((HWND)hwnd,(0x1000+155),0,pGroupMetrics); }
#undef ListView_SetHotCursor
__declspec(dllexport) int ListView_SetHotCursor(hwnd, hcur) { return SendMessage((HWND)hwnd,(0x1000+62),0,hcur); }
#undef ListView_SetHotItem
__declspec(dllexport) int ListView_SetHotItem(hwnd, i) { return SendMessage((HWND)hwnd,(0x1000+60),i,0); }
#undef ListView_SetHoverTime
__declspec(dllexport) int ListView_SetHoverTime(hwndLV, dwHoverTimeMs) { return SendMessage((HWND)hwndLV,(0x1000+71),0,dwHoverTimeMs); }
#undef ListView_SetIconSpacing
__declspec(dllexport) int ListView_SetIconSpacing(hwndLV, cx, cy) { return SendMessage((HWND)hwndLV,(0x1000+53),0,(((((cx)&0xffff))|((((cy)&0xffff)))<<16))); }
#undef ListView_SetImageList
__declspec(dllexport) int ListView_SetImageList(hwnd, himl, iImageList) { return SendMessage((HWND)hwnd,(0x1000+3),iImageList,himl); }
#undef ListView_SetInfoTip
__declspec(dllexport) int ListView_SetInfoTip(hwndLV, plvInfoTip) { return SendMessage((HWND)hwndLV,(0x1000+173),0,plvInfoTip); }
#undef ListView_SetInsertMark
__declspec(dllexport) int ListView_SetInsertMark(hwnd, lvim) { return SendMessage((HWND)hwnd,(0x1000+166),0,lvim); }
#undef ListView_SetInsertMarkColor
__declspec(dllexport) int ListView_SetInsertMarkColor(hwnd, color) { return SendMessage((HWND)hwnd,(0x1000+170),0,color); }
#undef ListView_SetItem
__declspec(dllexport) int ListView_SetItem(hwnd, pitem) { return SendMessage((HWND)hwnd,(0x1000+6),0,(pitem)); }
#undef ListView_SetItemCount
__declspec(dllexport) int ListView_SetItemCount(hwndLV, cItems) { return SendMessage((HWND)hwndLV,(0x1000+47),cItems,0); }
#undef ListView_SetItemCountEx
__declspec(dllexport) int ListView_SetItemCountEx(hwndLV, cItems, dwFlags) { return SendMessage((HWND)hwndLV,(0x1000+47),cItems,dwFlags); }
#undef ListView_SetItemPosition
__declspec(dllexport) int ListView_SetItemPosition(hwndLV, i, x, y) { return SendMessage((HWND)hwndLV,(0x1000+15),i,(((((((x))&0xffff))|(((((y))&0xffff)))<<16)))); }
#undef ListView_SetOutlineColor
__declspec(dllexport) int ListView_SetOutlineColor(hwnd, color) { return SendMessage((HWND)hwnd,(0x1000+177),0,color); }
#undef ListView_SetSelectedColumn
__declspec(dllexport) int ListView_SetSelectedColumn(hwnd, iCol) { return SendMessage((HWND)hwnd,(0x1000+140),iCol,0); }
#undef ListView_SetSelectionMark
__declspec(dllexport) int ListView_SetSelectionMark(hwnd, i) { return SendMessage((HWND)hwnd,(0x1000+67),0,i); }
#undef ListView_SetTextBkColor
__declspec(dllexport) int ListView_SetTextBkColor(hwnd, clrTextBk) { return SendMessage((HWND)hwnd,(0x1000+38),0,clrTextBk); }
#undef ListView_SetTextColor
__declspec(dllexport) int ListView_SetTextColor(hwnd, clrText) { return SendMessage((HWND)hwnd,(0x1000+36),0,clrText); }
#undef ListView_SetTileInfo
__declspec(dllexport) int ListView_SetTileInfo(hwnd, pti) { return SendMessage((HWND)hwnd,(0x1000+164),0,pti); }
#undef ListView_SetTileViewInfo
__declspec(dllexport) int ListView_SetTileViewInfo(hwnd, ptvi) { return SendMessage((HWND)hwnd,(0x1000+162),0,ptvi); }
#undef ListView_SetToolTips
__declspec(dllexport) int ListView_SetToolTips(hwndLV, hwndNewHwnd) { return SendMessage((HWND)hwndLV,(0x1000+74),hwndNewHwnd,0); }
#undef ListView_SetUnicodeFormat
__declspec(dllexport) int ListView_SetUnicodeFormat(hwnd, fUnicode) { return SendMessage((HWND)hwnd,(0x2000+5),fUnicode,0); }
#undef ListView_SetView
__declspec(dllexport) int ListView_SetView(hwnd, iView) { return SendMessage((HWND)hwnd,(0x1000+142),iView,0); }
#undef ListView_SetWorkAreas
__declspec(dllexport) int ListView_SetWorkAreas(hwnd, nWorkAreas, prc) { return SendMessage((HWND)hwnd,(0x1000+65),nWorkAreas,prc); }
#undef ListView_SortGroups
__declspec(dllexport) int ListView_SortGroups(hwnd, _pfnGroupCompate, _plv) { return SendMessage((HWND)hwnd,(0x1000+158),_pfnGroupCompate,_plv); }
#undef ListView_SortItems
__declspec(dllexport) int ListView_SortItems(hwndLV, _pfnCompare, _lPrm) { return SendMessage((HWND)hwndLV,(0x1000+48),_lPrm,_pfnCompare); }
#undef ListView_SortItemsEx
__declspec(dllexport) int ListView_SortItemsEx(hwndLV, _pfnCompare, _lPrm) { return SendMessage((HWND)hwndLV,(0x1000+81),_lPrm,_pfnCompare); }
#undef ListView_SubItemHitTest
__declspec(dllexport) int ListView_SubItemHitTest(hwnd, plvhti) { return SendMessage((HWND)hwnd,(0x1000+57),0,plvhti); }
#undef ListView_SubItemHitTestEx
__declspec(dllexport) int ListView_SubItemHitTestEx(hwnd, plvhti) { return SendMessage((HWND)hwnd,(0x1000+57),-1,plvhti); }
#undef ListView_Update
__declspec(dllexport) int ListView_Update(hwndLV, i) { return SendMessage((HWND)hwndLV,(0x1000+42),i,0); }
#undef MonthCal_GetColor
__declspec(dllexport) int MonthCal_GetColor(hmc, iColor) { return SendMessage((HWND)hmc,(0x1000+11),iColor,0); }
#undef MonthCal_GetCurSel
__declspec(dllexport) int MonthCal_GetCurSel(hmc, pst) { return SendMessage((HWND)hmc,(0x1000+1),0,pst); }
#undef MonthCal_GetFirstDayOfWeek
__declspec(dllexport) int MonthCal_GetFirstDayOfWeek(hmc) { return SendMessage((HWND)hmc,(0x1000+16),0,0); }
#undef MonthCal_GetMaxSelCount
__declspec(dllexport) int MonthCal_GetMaxSelCount(hmc) { return SendMessage((HWND)hmc,(0x1000+3),0,0); }
#undef MonthCal_GetMaxTodayWidth
__declspec(dllexport) int MonthCal_GetMaxTodayWidth(hmc) { return SendMessage((HWND)hmc,(0x1000+21),0,0); }
#undef MonthCal_GetMinReqRect
__declspec(dllexport) int MonthCal_GetMinReqRect(hmc, prc) { return SendMessage((HWND)hmc,(0x1000+9),0,prc); }
#undef MonthCal_GetMonthDelta
__declspec(dllexport) int MonthCal_GetMonthDelta(hmc) { return SendMessage((HWND)hmc,(0x1000+19),0,0); }
#undef MonthCal_GetMonthRange
__declspec(dllexport) int MonthCal_GetMonthRange(hmc, gmr, rgst) { return SendMessage((HWND)hmc,(0x1000+7),gmr,rgst); }
#undef MonthCal_GetRange
__declspec(dllexport) int MonthCal_GetRange(hmc, rgst) { return SendMessage((HWND)hmc,(0x1000+17),0,rgst); }
#undef MonthCal_GetSelRange
__declspec(dllexport) int MonthCal_GetSelRange(hmc, rgst) { return SendMessage((HWND)hmc,(0x1000+5),0,rgst); }
#undef MonthCal_GetToday
__declspec(dllexport) int MonthCal_GetToday(hmc, pst) { return SendMessage((HWND)hmc,(0x1000+13),0,pst); }
#undef MonthCal_GetUnicodeFormat
__declspec(dllexport) int MonthCal_GetUnicodeFormat(hwnd) { return SendMessage((HWND)hwnd,(0x2000+6),0,0); }
#undef MonthCal_HitTest
__declspec(dllexport) int MonthCal_HitTest(hmc, pinfo) { return SendMessage((HWND)hmc,(0x1000+14),0,pinfo); }
#undef MonthCal_SetColor
__declspec(dllexport) int MonthCal_SetColor(hmc, iColor, clr) { return SendMessage((HWND)hmc,(0x1000+10),iColor,clr); }
#undef MonthCal_SetCurSel
__declspec(dllexport) int MonthCal_SetCurSel(hmc, pst) { return SendMessage((HWND)hmc,(0x1000+2),0,pst); }
#undef MonthCal_SetDayState
__declspec(dllexport) int MonthCal_SetDayState(hmc, cbds, rgds) { return SendMessage((HWND)hmc,(0x1000+8),cbds,rgds); }
#undef MonthCal_SetFirstDayOfWeek
__declspec(dllexport) int MonthCal_SetFirstDayOfWeek(hmc, iDay) { return SendMessage((HWND)hmc,(0x1000+15),0,iDay); }
#undef MonthCal_SetMaxSelCount
__declspec(dllexport) int MonthCal_SetMaxSelCount(hmc, n) { return SendMessage((HWND)hmc,(0x1000+4),n,0); }
#undef MonthCal_SetMonthDelta
__declspec(dllexport) int MonthCal_SetMonthDelta(hmc, n) { return SendMessage((HWND)hmc,(0x1000+20),n,0); }
#undef MonthCal_SetRange
__declspec(dllexport) int MonthCal_SetRange(hmc, gd, rgst) { return SendMessage((HWND)hmc,(0x1000+18),gd,rgst); }
#undef MonthCal_SetSelRange
__declspec(dllexport) int MonthCal_SetSelRange(hmc, rgst) { return SendMessage((HWND)hmc,(0x1000+6),0,rgst); }
#undef MonthCal_SetToday
__declspec(dllexport) int MonthCal_SetToday(hmc, pst) { return SendMessage((HWND)hmc,(0x1000+12),0,pst); }
#undef MonthCal_SetUnicodeFormat
__declspec(dllexport) int MonthCal_SetUnicodeFormat(hwnd, fUnicode) { return SendMessage((HWND)hwnd,(0x2000+5),fUnicode,0); }
#undef Pager_ForwardMouse
__declspec(dllexport) int Pager_ForwardMouse(hwnd, bForward) { return SendMessage((HWND)hwnd,(0x1400+3),bForward,0); }
#undef Pager_GetBkColor
__declspec(dllexport) int Pager_GetBkColor(hwnd) { return SendMessage((HWND)hwnd,(0x1400+5),0,0); }
#undef Pager_GetBorder
__declspec(dllexport) int Pager_GetBorder(hwnd) { return SendMessage((HWND)hwnd,(0x1400+7),0,0); }
#undef Pager_GetButtonSize
__declspec(dllexport) int Pager_GetButtonSize(hwnd) { return SendMessage((HWND)hwnd,(0x1400+11),0,0); }
#undef Pager_GetButtonState
__declspec(dllexport) int Pager_GetButtonState(hwnd, iButton) { return SendMessage((HWND)hwnd,(0x1400+12),0,iButton); }
#undef Pager_GetDropTarget
__declspec(dllexport) int Pager_GetDropTarget(hwnd, ppdt) { return SendMessage((HWND)hwnd,(0x2000+4),0,ppdt); }
#undef Pager_GetPos
__declspec(dllexport) int Pager_GetPos(hwnd) { return SendMessage((HWND)hwnd,(0x1400+9),0,0); }
#undef Pager_RecalcSize
__de