dll "qm.exe" #FileGetAttributesTimeout $_file timeoutMS [flags]

str sf="\\gintaras\Q\app\ok.qml"
 str sf="$my qm$\test\ok.qml"
 out GetAttr(sf)

PF
int a=FileGetAttributesTimeout(sf 1000)
PN;PO
outx a
