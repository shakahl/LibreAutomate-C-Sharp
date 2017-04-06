 /
function $path [flags] ;;flags: 1 autorestore.

 Changes current directory.

 path - new current directory.
 flags:
   1 - restore previous current directory when macro ends.

 REMARKS
 Setting current directory is not reliable because it is shared by all threads.
 Use this only when it is necessary.
 Don't use in threads that may run simultaneously.

 Added in: QM 2.3.0.


if(flags&1) str- ___chdir=GetCurDir; atend sub.Atend

if(!SetCurrentDirectoryW(@_s.expandpath(path))) end "Cannot change current directory" 16


#sub Atend
str- ___chdir
SetCurDir ___chdir
