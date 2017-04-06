 /
function $from $to [flags] ;;flags: 1 error if exists, 2 to is not a destination folder

 Moves file or folder.
 Error if fails.
 Faster than ren but does not have many of ren features. See <help>FileCopy</help>.

 from - full path of the source file or folder.
 to - full path of the destination file or folder. If it is an existing folder, moves into it, unless flag 2 used.
 flags:
   1 - error if the destination file already exists. Without this flag, at first deletes it.
   2 - if to is existing folder, replace it instead of moving to it.

 Added in: QM 2.3.0.


opt noerrorshere 1
str s1 s2
sub_sys.FileCopyMove_Prepare(from to flags s1 s2)
if(!MoveFileExW(@s1 @s2 MOVEFILE_COPY_ALLOWED)) end ERR_FILERENAME 16
