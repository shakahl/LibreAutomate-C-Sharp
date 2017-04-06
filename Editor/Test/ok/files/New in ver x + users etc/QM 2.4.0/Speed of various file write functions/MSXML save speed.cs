 out
str sfs="C:\Users\G\AppData\Local\Microsoft\WebSetup\PreCopiedMediaSources\globalinstallorder.xml"
 str sfs="C:\Users\G\AppData\Roaming\GinDi\Quick Macros\iconcache16.xml"
str sf.expandpath("$my qm$\test\safe.xml")
PF
 _s.getfile(sfs); PN; if(_s.beg("[0xef][0xbb][0xbf]")) _s.remove(0 3)
 out _s.len; outb _s 8

 typelib MSXML2 {F5078F18-C551-11D3-89B9-0000F81FE221} 3.0
typelib MSXML2 {F5078F18-C551-11D3-89B9-0000F81FE221} 6.0
MSXML2.DOMDocument doc._create
if(!doc.load(sfs)) end "failed"
 doc.loadXML(_s)
PN
 PN
doc.save(sf)
PN;PO
 out GetFileFragmentation(sf)
