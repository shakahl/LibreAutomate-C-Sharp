function $fileOld $fileNew $fileDiff [flags] ;;flags: 1 Rabin's algorithm

 Makes binary diff of 2 files.
 Everything is as with <tip>Xdiff.DiffBinary</tip>.
 fileDiff receives results.


str s1.getfile(fileOld) s2.getfile(fileNew) s3

FileHeader(fileOld fileNew s3)

DiffBinary(s1 s2 s3 flags)

s3.setfile(fileDiff)

err+ end _error
