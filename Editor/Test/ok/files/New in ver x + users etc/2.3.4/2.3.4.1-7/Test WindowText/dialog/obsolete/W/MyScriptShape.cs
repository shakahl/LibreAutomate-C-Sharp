 /dlg_apihook
function# hdc !**psc @*pwcChars cChars cMaxGlyphs SCRIPT_ANALYSIS*psa @*pwOutGlyphs @*pwLogClust SCRIPT_VISATTR*psva *pcGlyphs

int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
int R=call(fnScriptShape hdc psc pwcChars cChars cMaxGlyphs psa pwOutGlyphs pwLogClust psva pcGlyphs)
t_inAPI-1; if(inAPI and !t_all) ret R

 ret R
if(R) ret R

RECT r

CommonTextFunc 9 hdc pwcChars cChars r

ret R
