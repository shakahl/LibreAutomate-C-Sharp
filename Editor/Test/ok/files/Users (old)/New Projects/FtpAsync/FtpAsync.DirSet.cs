function# $directory

 Sets current ftp directory.
 directory can be relative to current directory, or full path. Can be case sensitive.
 Examples of relative: "pub", ".." (one level up).
 Examples of full: "/pub/rfc", "/" (root).


if(empty(directory)) directory=".."

if(FtpSetCurrentDirectoryW(m_hi @directory)) ret 1
Error
