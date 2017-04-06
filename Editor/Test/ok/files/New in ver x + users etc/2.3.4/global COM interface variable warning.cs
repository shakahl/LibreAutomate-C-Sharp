IUnknown+ g_unk
IStringMap+ g_sm
Acc+ g_acc
ARRAY(IUnknown)+ g_au
ARRAY(IStringMap)+ g_asm
ARRAY(Acc)+ g_aacc

type TUNK i IUnknown'u
TUNK+ g_tunk
type TARR i ARRAY(IUnknown)u
TARR+ g_tarr
type TSM i IStringMap'u
TSM+ g_tsm
type TARRSM i ARRAY(IStringMap)u
TARRSM+ g_tarrsm

IUnknown+* g_up
IUnknown+& g_ur

 =acc(mouse)
 g_acc.Role(_s); out _s
