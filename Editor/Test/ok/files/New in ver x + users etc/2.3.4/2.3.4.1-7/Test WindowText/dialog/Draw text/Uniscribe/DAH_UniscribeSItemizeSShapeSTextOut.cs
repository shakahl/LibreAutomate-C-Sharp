 /dlg_apihook
function hdc BSTR's CLogicalCoord&c

int hr
 __________________________________

byte* psc pss ssc;; SCRIPT_CONTROL SCRIPT_STATE SCRIPT_CACHE
ARRAY(WINAPI2.SCRIPT_ITEM) items.create(100); int i cItems
 hr=WINAPI2.ScriptItemize(s s.len items.len +psc +pss &items[0] &cItems)
hr=WINAPI2.ScriptItemize(s s.len items.len 0 0 &items[0] &cItems)
 out hr
int dx(c.x) dy(c.y)
for i 0 cItems
	int idx = items[i].iCharPos
	int lll = items[i+1].iCharPos - idx;
	int mx_glyph = lll * 2 + 16;
	int glyph_cnt = 0;

	word* clusts =malloc(2*mx_glyph);
	word* glyphs =malloc(2*mx_glyph);
	WINAPI2.SCRIPT_VISATTR* sva =malloc(sizeof(WINAPI2.SCRIPT_VISATTR)*mx_glyph);
	int* adv=0;
	WINAPI2.GOFFSET* offs=0;

	 Shape
	hr = WINAPI2.ScriptShape(hdc, &ssc, s.pstr+(2*idx), lll, mx_glyph, &items[i].a, glyphs, clusts, sva, &glyph_cnt);
	if(hr) goto clean;

	WINAPI2.ABC abc;
	offs = malloc(sizeof(WINAPI2.GOFFSET)*mx_glyph);
	adv =malloc(sizeof(int)*mx_glyph);

	 Place
	hr = WINAPI2.ScriptPlace(hdc, &ssc, glyphs, glyph_cnt, sva, &items[i].a, adv, offs, &abc);
	if(hr) goto clean;

	 TextOut
	 offs[0].dv+40
	 offs[0].du+8
	int* just =malloc(4*mx_glyph); WINAPI2.ScriptJustify(sva adv glyph_cnt 8 0 just)
	 for(_i 0 glyph_cnt) out "%i %i %i" adv[_i] just[_i] offs[_i].du
	 hr = ScriptTextOut(hdc, &ssc, dx, dy, 0, 0, &items[i].a, 0, 0, glyphs, glyph_cnt, adv, just, offs);
	
	hr = WINAPI2.ScriptTextOut(hdc, &ssc, dx, dy, 0, 0, &items[i].a, 0, 0, glyphs, glyph_cnt, adv, 0, offs);
	dx += abc.abcA + abc.abcB + abc.abcC; dy+8
	
	 clean
	free(clusts);
	free(glyphs);
	free(sva);
	free(adv);
	free(just);
	if(hr) break;

WINAPI2.ScriptFreeCache &ssc
