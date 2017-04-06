function volumePercent

 Sets volume.

 volumePercent - volume 0 - 100 (%).


opt noerrorshere
if(!_v) Init

FLOAT f=volumePercent/100.0
_v.SetMasterVolumeLevelScalar(f 0)
