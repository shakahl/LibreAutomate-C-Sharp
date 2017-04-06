dll- "qm.exe" DebugKwt *a
type DEBUG_KWT c1 c2 c3 c4 c5
DEBUG_KWT+ g_dkwt
DEBUG_KWT dkwt

if(!g_dkwt.c1)
	DebugKwt &g_dkwt.c1
	err tim
else
	DebugKwt &dkwt.c1
	if(memcmp(&g_dkwt &dkwt sizeof(DEBUG_KWT)))
		mes "Keyword tables modified.[][]Was: %i %i %i %i %i[]Now: %i %i %i %i %i" "" "x" g_dkwt.c1 g_dkwt.c2 g_dkwt.c3 g_dkwt.c4 g_dkwt.c5 dkwt.c1 dkwt.c2 dkwt.c3 dkwt.c4 dkwt.c5
		