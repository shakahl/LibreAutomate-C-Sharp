 /

 import registry
RegImportXml "$qm$\qmpe_reg.xml"; err ret

 replace something
str rks="Software\GinDi\QM2\settings"
str sfmyqm.expandpath("$qm$\Data\My QM")
rset _s.from(sfmyqm "\Main.qml") "file" rks
rset sfmyqm "my qm" rks
rset "" "backups" rks

ret 1