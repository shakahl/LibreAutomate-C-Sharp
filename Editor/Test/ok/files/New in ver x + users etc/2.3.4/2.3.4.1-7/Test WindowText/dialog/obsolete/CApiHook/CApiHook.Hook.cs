#ret
 /dlg_apihook
int+ hwndOutput(id(2201 _hwndqm)) hwndSB(id(2204 _hwndqm))

 int+ fnMessageBoxA=&MessageBox
 int+ fnMessageBoxW=&MessageBoxW
int+ fnExtTextOutW=&ExtTextOutW
int+ fnDrawTextExW=&DrawTextExW
int+ fnTextOutW=&TextOutW
int+ fnPolyTextOutW=&PolyTextOutW
int+ fnGdipDrawString=&GDIP.GdipDrawString; err
int+ fnGdipDrawDriverString=&GDIP.GdipDrawDriverString; err
int+ fnBitBlt=&BitBlt
int+ fnExtTextOutA=&ExtTextOut
int+ fnTextOutA=&TextOut
int+ fnPolyTextOutA=&PolyTextOut

type SCRIPT_STATE @bf1
type SCRIPT_ANALYSIS @bf1 SCRIPT_STATE's
type SCRIPT_VISATTR @bf1
type GOFFSET du dv
type OPENTYPE_FEATURE_RECORD tagFeature lParameter
type TEXTRANGE_PROPERTIES OPENTYPE_FEATURE_RECORD*potfRecords cotfRecords
type SCRIPT_CHARPROP @bf1
type SCRIPT_VISATTR @bf1
type SCRIPT_GLYPHPROP SCRIPT_VISATTR'sva @reserved
dll usp10 #ScriptShape hdc !**psc @*pwcChars cChars cMaxGlyphs SCRIPT_ANALYSIS*psa @*pwOutGlyphs @*pwLogClust SCRIPT_VISATTR*psva *pcGlyphs
dll- usp10 #ScriptShapeOpenType hdc !**psc SCRIPT_ANALYSIS*psa tagScript tagLangSys *rcRangeChars TEXTRANGE_PROPERTIES**rpRangeProperties cRanges @*pwcChars cChars cMaxGlyphs @*pwLogClust SCRIPT_CHARPROP*pCharProps @*pwOutGlyphs SCRIPT_GLYPHPROP*pOutGlyphProps *pcGlyphs
dll usp10 #ScriptTextOut hdc !**psc x y fuOptions RECT*lprc SCRIPT_ANALYSIS*psa @*pwcReserved iReserved @*pwGlyphs cGlyphs *piAdvance *piJustify GOFFSET*pGoffset
int+ fnScriptShape=&ScriptShape
#if _winnt>=6
int+ fnScriptShapeOpenType=&ScriptShapeOpenType
#endif
int+ fnScriptTextOut=&ScriptTextOut

 int+ fnDrawThemeText=&DrawThemeText
 int+ fnDrawThemeTextEx=&DrawThemeTextEx; err

 if(m_method=3 and MH_Initialize) end "failed to init"

 if(!__Hook(+&fnMessageBoxA &MyMessageBoxA)) end "failed to hook"
 if(!__Hook(+&fnMessageBoxW &MyMessageBoxW)) end "failed to hook"

if(!__Hook(+&fnExtTextOutW &MyExtTextOutW)) end "failed to hook ExtTextOutW"
if(!__Hook(+&fnDrawTextExW &MyDrawTextExW)) end "failed to hook DrawTextExW"
if(!__Hook(+&fnTextOutW &MyTextOutW)) end "failed to hook TextOutW"
if(!__Hook(+&fnPolyTextOutW &MyPolyTextOutW)) end "failed to hook PolyTextOutW" 
if fnGdipDrawString
	if(!__Hook(+&fnGdipDrawString &MyGdipDrawString)) end "failed to hook GdipDrawString"
	if(!__Hook(+&fnGdipDrawDriverString &MyGdipDrawDriverString)) end "failed to hook GdipDrawDriverString"
if(!__Hook(+&fnBitBlt &MyBitBlt)) end "failed to hook BitBlt"
if(!__Hook(+&fnExtTextOutA &MyExtTextOutA)) end "failed to hook ExtTextOutA"
if(!__Hook(+&fnTextOutA &MyTextOutA)) end "failed to hook TextOutA"
if(!__Hook(+&fnPolyTextOutA &MyPolyTextOutA)) end "failed to hook PolyTextOutA"
if(!__Hook(+&fnScriptShape &MyScriptShape)) end "failed to hook ScriptShape"
#if _winnt>=6
if(!__Hook(+&fnScriptShapeOpenType &MyScriptShapeOpenType)) end "failed to hook ScriptShapeOpenType"
#endif
if(!__Hook(+&fnScriptTextOut &MyScriptTextOut)) end "failed to hook ScriptTextOut"

 if(!__Hook(+&fnDrawThemeText &MyDrawThemeText)) end "failed to hook"
 if(!__Hook(+&fnDrawThemeTextEx &MyDrawThemeTextEx)) end "failed to hook"

if(m_method=5)
	if(AH_EnableHooks) out "failed to enable hooks"

m_hooked=1
