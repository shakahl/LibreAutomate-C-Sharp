function#

 Gets volume.
 Returns 0 - 100 (%).


opt noerrorshere
if(!_v) Init

FLOAT f
_v.GetMasterVolumeLevelScalar(f)
ret Round(f*100.0)
