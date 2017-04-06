str sf="$my qm$\test\ok.qml"
 str sf="$qm$\system.qml"

PF
 __HFile f.Create(sf OPEN_EXISTING GENERIC_READ FILE_SHARE_READ|FILE_SHARE_WRITE)
__HFile f.Create(sf OPEN_EXISTING GENERIC_READ FILE_SHARE_READ); err
PN
PO
out f
