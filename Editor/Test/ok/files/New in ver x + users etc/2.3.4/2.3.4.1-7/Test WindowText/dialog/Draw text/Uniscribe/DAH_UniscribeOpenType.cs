 /dlg_apihook
function hdc BSTR's CLogicalCoord&c

#if _winnt>=6

int hr
int glen=2*s.len+16

WINAPI2.SCRIPT_CONTROL* psControl
WINAPI2.SCRIPT_STATE* psState
int cMaxItems = 500;
ARRAY(WINAPI2.SCRIPT_ITEM) items.create(500);
int cItems;

 Itemize
int* scrpt =malloc(sizeof(int)*s.len);

hr = WINAPI2.ScriptItemizeOpenType(s, s.len, cMaxItems, psControl, +psState, &items[0], scrpt, &cItems);

byte* ssc
int i dx(c.x) dy(c.y)

for i 0 cItems
	int idx = items[i].iCharPos
	int lll = items[i+1].iCharPos - idx
	int mx_glyph = lll * 2 + 16
	
	int glyph_cnt=0;
	
	word* clusts =malloc(2*mx_glyph);
	word* glyphs =malloc(2*mx_glyph);
	int* adv=0;
	WINAPI2.GOFFSET* offs=0;
	
	 Set Tag
	int tag78 = 'j' | ('p' << 8) | ('7' << 16) | ('8' << 24);
	int tag83 = 'j' | ('p' << 8) | ('8' << 16) | ('3' << 24);
	int tag90 = 'j' | ('p' << 8) | ('9' << 16) | ('0' << 24);
	int tag04 = 'j' | ('p' << 8) | ('0' << 16) | ('4' << 24);
	WINAPI2.OPENTYPE_FEATURE_RECORD rcd;
	WINAPI2.TEXTRANGE_PROPERTIES* prop; WINAPI2.TEXTRANGE_PROPERTIES pp; prop = &pp;
	
	WINAPI2.SCRIPT_CHARPROP* charProp =malloc(sizeof(WINAPI2.SCRIPT_CHARPROP)*lll);
	WINAPI2.SCRIPT_GLYPHPROP* glyhProp =malloc(sizeof(WINAPI2.SCRIPT_GLYPHPROP)*lll);
	int* range =malloc(sizeof(int)*lll);
	for(int'kk 0 lll) range[kk]=0;
	
	rcd.tagFeature = tag04;
	rcd.lParameter = 1;
	pp.potfRecords = &rcd;
	pp.cotfRecords = 1;

	 sel(md)
	 case ID_VIEW_JP78: rcd.tagFeature = tag78
	 case ID_VIEW_JP83: rcd.tagFeature = tag83
	 case ID_VIEW_JP90: rcd.tagFeature = tag90
	 case ID_VIEW_JP04: rcd.tagFeature = tag04
	rcd.tagFeature = tag78

	 Shape
	hr = WINAPI2.ScriptShapeOpenType(hdc, &ssc, &items[i].a, scrpt[i], 0, &lll, &prop, 1, s.pstr+idx, lll, mx_glyph, clusts, charProp, glyphs, glyhProp, &glyph_cnt);
	if(hr) goto clean;

	WINAPI2.ABC abc;
	offs =malloc(sizeof(WINAPI2.GOFFSET)*mx_glyph);
	adv =malloc(sizeof(int)*mx_glyph);

	 Place
	hr = WINAPI2.ScriptPlaceOpenType(hdc, &ssc, &items[i].a, scrpt[i], 0, &lll, &prop, 1, s.pstr+idx, clusts, charProp, lll, glyphs, glyhProp, glyph_cnt, adv, offs, &abc);
	if(hr) goto clean;

	 TextOut
	hr = WINAPI2.ScriptTextOut(hdc, &ssc, dx, dy, 0, 0, &items[i].a, 0, 0, glyphs, glyph_cnt, adv, 0, offs);
	dx += abc.abcA + abc.abcB + abc.abcC;

  clean
	free(clusts);
	free(glyphs);
	free(adv);
	free(charProp);
	free(glyhProp);
	free(range);
	if(hr) break;

free(scrpt);
WINAPI2.ScriptFreeCache(&ssc);
