dll usp10 #ScriptCacheGetHeight hdc !**psc *tmHeight
dll usp10 #ScriptGetCMap hdc !**psc @*pwcInChars cChars dwFlags @*pwOutGlyphs
dll usp10 #ScriptGetFontAlternateGlyphs hdc !**psc SCRIPT_ANALYSIS*psa tagScript tagLangSys tagFeature @wGlyphId cMaxAlternates @*pAlternateGlyphs *pcAlternates
dll usp10 #ScriptGetFontFeatureTags hdc !**psc SCRIPT_ANALYSIS*psa tagScript tagLangSys cMaxTags *pFeatureTags *pcTags
dll usp10 #ScriptGetFontLanguageTags hdc !**psc SCRIPT_ANALYSIS*psa tagScript cMaxTags *pLangsysTags *pcTags
dll usp10 #ScriptGetFontProperties hdc !**psc SCRIPT_FONTPROPERTIES*sfp
dll usp10 #ScriptGetFontScriptTags hdc !**psc SCRIPT_ANALYSIS*psa cMaxTags *pScriptTags *pcTags
dll usp10 #ScriptGetGlyphABCWidth hdc !**psc @wGlyph ABC*pABC
dll usp10 #ScriptPlace hdc !**psc @*pwGlyphs cGlyphs SCRIPT_VISATTR*psva SCRIPT_ANALYSIS*psa *piAdvance GOFFSET*pGoffset ABC*pABC
dll usp10 #ScriptPlaceOpenType hdc !**psc SCRIPT_ANALYSIS*psa tagScript tagLangSys *rcRangeChars TEXTRANGE_PROPERTIES**rpRangeProperties cRanges @*pwcChars @*pwLogClust SCRIPT_CHARPROP*pCharProps cChars @*pwGlyphs SCRIPT_GLYPHPROP*pGlyphProps cGlyphs *piAdvance GOFFSET*pGoffset ABC*pABC
dll usp10 #ScriptPositionSingleGlyph hdc !**psc SCRIPT_ANALYSIS*psa tagScript tagLangSys tagFeature lParameter @wGlyphId iAdvance GOFFSET'GOffset *piOutAdvance GOFFSET*pOutGoffset
dll usp10 #ScriptShape hdc !**psc @*pwcChars cChars cMaxGlyphs SCRIPT_ANALYSIS*psa @*pwOutGlyphs @*pwLogClust SCRIPT_VISATTR*psva *pcGlyphs
dll usp10 #ScriptShapeOpenType hdc !**psc SCRIPT_ANALYSIS*psa tagScript tagLangSys *rcRangeChars TEXTRANGE_PROPERTIES**rpRangeProperties cRanges @*pwcChars cChars cMaxGlyphs @*pwLogClust SCRIPT_CHARPROP*pCharProps @*pwOutGlyphs SCRIPT_GLYPHPROP*pOutGlyphProps *pcGlyphs
dll usp10 #ScriptStringAnalyse hdc !*pString cString cGlyphs iCharset dwFlags iReqWidth SCRIPT_CONTROL*psControl SCRIPT_STATE*psState *piDx SCRIPT_TABDEF*pTabdef !*pbInClass !**pssa
dll usp10 #ScriptSubstituteSingleGlyph hdc !**psc SCRIPT_ANALYSIS*psa tagScript tagLangSys tagFeature lParameter @wGlyphId @*pwOutGlyphId
dll usp10 #ScriptTextOut hdc !**psc x y fuOptions RECT*lprc SCRIPT_ANALYSIS*psa @*pwcReserved iReserved @*pwGlyphs cGlyphs *piAdvance *piJustify GOFFSET*pGoffset
