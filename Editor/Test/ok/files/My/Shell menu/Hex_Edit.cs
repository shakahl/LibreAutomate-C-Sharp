function $files
str f
foreach f files
	 run "$program files$\HexEdit\HexEdit.exe" F"''{f}''" "" "*"
	run "C:\Program Files\HHD Software\Hex Editor Neo\HexFrame.exe" F"''{f}''" "" "*"
