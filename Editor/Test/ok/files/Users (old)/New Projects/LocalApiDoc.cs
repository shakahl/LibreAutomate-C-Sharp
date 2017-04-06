 Coud be used to quickly jump to local MSDN or SDK help, but does not work with new Document Explorer


str s=_command; if(!s.len) ret
typelib VsHelp {83285928-227C-11D3-B870-00C04F79F802} 1.0
#err

#ifdef VsHelp

VsHelp.DExploreAppObj dex._getactive; err goto ge
ARRAY(str) a
if(tok(s a 2 " " 2)>1)
	dex.Filter=a[1]; err
	a[0]+" " ;;1. Does not allow to open a topic (would not open anyway if a[0] does not exactly match an item in the index). 2. The item in index often is like "GetTickCount function [Windows]", and often preceded by "GetTickCount" item that we don't need, so appending space will select good item.
	dex.DisplayTopicFromKeyword(a[0]); err
	err-
	int hwnd=win("" "wndclass_desked_gsk")
	int hin=child("" "hx_winclass_vlist" hwnd)
	Acc ai=acc("Help Index Control" "LIST" hin "hx_winclass_vlist" "" 0x1001)
	ai.Selection
	s=ai.Name
	if(s.beg(a[0].rtrim)) dex.DisplayTopicFromKeyword(s); err
	err+
else
	dex.DisplayTopicFromF1Keyword(a[0]); err
	dex.SyncIndex(a[0] 0); err
ret

#endif
 ge
mes "Microsoft Document Explorer (dexplore.exe) must be installed and running, and ''MSDN Library'', ''Platform SDK'' or other collection loaded. MSDN Library is included with Microsoft Visual Studio. If you don't have it, you can download MSDN library from Microsoft. Or you can download Platform SDK, which includes mostly used Windows API documentation. Platform SDK is smaller, and you can download only documentation, not whole SDK." "" "i"
