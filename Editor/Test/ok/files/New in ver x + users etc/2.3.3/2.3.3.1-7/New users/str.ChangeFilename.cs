function $newFilename

 Replaces filename. Does not change extension.

 newFilename - new filename. Must be without extension.

 REMARKS
 This variable initially can contain filename with extension, or full file path. Can be enclosed in ".

 EXAMPLE
 str f="c:\f\A.gif"
 f.ChangeFilename("B")
 out f


_s.from(newFilename "$1")
this.replacerx("(?<=\\|^)[^\\/|:''\r\n]+(\.[^\.\\/|:\s]+)$" _s 4)
