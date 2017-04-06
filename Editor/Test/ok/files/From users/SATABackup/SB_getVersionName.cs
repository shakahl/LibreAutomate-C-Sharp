function~ ~F1 ~Path [#Version] [#Length]
;; Input:
 	F1: Original Filename with relative path
 	Path: Full Path for F1
 	Version: Version to add to filename
 	Length: Number of character for version. Only optional if Version not included.
;; Output
 	Returns filename with version

if(!Version) Version=0
str a b c name
if (Version = 0)
	name.format("%s%s" Path F1)
else
	a.getPathFileName(F1)
	b.GetFilenameExt(F1)
	if (b.len>0)
		name.format("%s%s%s%s%s%s" Path a ".v" c.intFixLength(Version Length) "." b)
	else
		name.format("%s%s%s%s" Path a ".v" c.intFixLength(Version Length))

ret name