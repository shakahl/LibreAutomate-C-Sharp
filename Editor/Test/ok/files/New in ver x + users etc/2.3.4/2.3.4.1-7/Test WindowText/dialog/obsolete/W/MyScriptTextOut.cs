 /dlg_apihook
function# hdc !**psc x y fuOptions RECT*lprc SCRIPT_ANALYSIS*psa @*pwcReserved iReserved @*pwGlyphs cGlyphs *piAdvance *piJustify GOFFSET*pGoffset

 just eats ExtTextOut

int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
 out "before ScriptTextOut, inAPI=%i, hdc=%i" inAPI hdc
int R=call(fnScriptTextOut hdc psc x y fuOptions lprc psa pwcReserved iReserved pwGlyphs cGlyphs piAdvance piJustify pGoffset)
 out "after ScriptTextOut"
t_inAPI-1; if(inAPI and !t_all) ret R

ret R
