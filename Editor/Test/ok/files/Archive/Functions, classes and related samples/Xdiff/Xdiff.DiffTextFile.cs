function $fileOld $fileNew $fileDiff [nContextLines] [flags] ;;flags: 2 minimal

 Makes unified diff of 2 text files.
 Everything except flag 1 is as with <tip>Xdiff.DiffText</tip>.
 fileDiff receives results.


str s1.getfile(fileOld) s2.getfile(fileNew) s3

FileHeader(fileOld fileNew s3)

DiffText(s1 s2 s3 nContextLines flags|1)

s3.setfile(fileDiff)

err+ end _error
