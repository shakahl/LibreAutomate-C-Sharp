str sfs="C:\Users\G\AppData\Local\Microsoft\WebSetup\PreCopiedMediaSources\globalinstallorder.xml"
 str sfs="C:\Users\G\AppData\Roaming\GinDi\Quick Macros\iconcache16.xml"
str sf.expandpath("$my qm$\test\safe.xml")
IXml x._create
PF
 _s.getfile(sfs); PN; if(_s.beg("[0xef][0xbb][0xbf]")) _s.remove(0 3)
 x.FromString(_s)
x.FromFile(sfs)
PN
x.ToFile(sf)
 _s.flags=3; x.ToString(_s); PN; _s.setfile(sf 0 -1 8)
PN;PO
out GetFileFragmentation(sf)

 speed: 55279  48622  (string, 1 frag)
 speed: 55897  258135  (file, 10-33 frag)

 speed: 56878  30163  (file, 3-7 frag)

 IXml xx._create
 xx.FromFile(sf)
 xx.ToString(_s); out _s
