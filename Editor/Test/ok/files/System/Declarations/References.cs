 All these type libraries and refs are loaded on demand.

#opt hidedecl 1
typelib stdole {00020430-0000-0000-C000-000000000046} 2.0 0 1
typelib MailBee "$qm$\MailBee.dll" 1
#opt hidedecl 0
typelib Services "$qm$\ARServicesMgr.DLL" 1
typelib MSHTML {3050F1C5-98B5-11CF-BB82-00AA00BDCE0B} 4.0 0 1
typelib SHDocVw {EAB22AC0-30C1-11CF-A7EB-0000C05BAE0B} 1.1 0 1
typelib Wsh {F935DC20-1CF0-11D0-ADB9-00C04FD58A0B} 1.0 0 1
typelib Shell32 {50A7E9B0-70EF-11D1-B75A-00A0C90564FE} 1.0 0 1
typelib MSScript "$system$\msscript.ocx" 1
typelib Excel {00020813-0000-0000-C000-000000000046} 1.2 0 1
#err __err_Excel
typelib ADO {00000205-0000-0010-8000-00AA006D2EA4} 2.5 0 1
typelib UIA {944DE083-8FB8-45CF-BCB7-C477ACB2F897} 1.0 0 1

ref WINAPI "$qm$\winapi.txt" 7 ;;Windows XP/2003 API.
ref WINAPIV "$qm$\winapiv.txt" 1 ;;Windows Vista API.
ref WINAPI7 "$qm$\winapi7.txt" 1 ;;Windows 7 API. Added in QM 2.3.1.
ref GDIP "$qm$\gdiplus.txt" 1 ;;GDI+ flat API. Added in QM 2.3.2.
#opt hidedecl 1
ref SCI "$qm$\scintilla.txt" 1 ;;Scintilla API. Can be used with QM code editor and some other controls, and with controls of "Scintilla" class.
ref GRID "__qm_grid_api" 1 ;;QM_Grid control API. Used by DlgGrid class. Added in QM 2.3.2.
ref __sqlite "$qm$\sqlite.txt" 1 ;;SQLite API. Used by Sqlite class. Added in QM 2.3.2.
ref __tcc "__tcc_api" 1 ;;TCC API. Used by __Tcc class. Added in QM 2.3.2.
ref FFDOM "__ffdom_api" 1 ;;Gecko (Firefox etc) simple DOM interfaces. Used by FFNode class. Added in QM 2.3.3.
ref __obsolete "obsolete decl" 3
#opt hidedecl 0
