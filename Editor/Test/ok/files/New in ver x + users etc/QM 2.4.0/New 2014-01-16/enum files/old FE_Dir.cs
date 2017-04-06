out
Dir_old d
 foreach d "Q:\My QM\test\*" FE_Dir_old 2|4
 foreach d "Q:\My QM\test\*.qml" FE_Dir_old 2|4
 foreach d "Q:\My QM\test\export.qmL" FE_Dir_old 4
foreach d "*.qml" FE_Dir_old
	out d.FileName(1)
