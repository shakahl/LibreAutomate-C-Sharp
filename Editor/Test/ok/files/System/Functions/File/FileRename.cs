 /
function $from $newname [flags] ;;flags: 1 error if exists

 Renames file or folder.
 Error if fails.
 Faster than ren* but does not have many of ren features. See <help>FileCopy</help>.

 from - full path of the file or folder.
 newname - new filename (not full path).
 flags:
   1 - error if the destination file already exists. Without this flag, at first deletes it.

 Added in: QM 2.3.0.


opt noerrorshere 1
str s1.getpath(from)
s1+newname
FileMove from s1 flags|2
