function $fileOld $fileNew1 $fileNew2 $fileMerged [str&sRejected] ;;flags: 1 reverse, 0x100 ignore whitespace

 Merges 3 text files.
 Everything is as with <tip>Xdiff.Merge3Text</tip>.
 fileMerged receives results.


str s0.getfile(fileOld) s1.getfile(fileNew1) s2.getfile(fileNew2) s3

Merge3Text(s0 s1 s2 s3 sRejected)

s3.setfile(fileMerged)

err+ end _error
