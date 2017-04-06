function $fileOld $fileDiff $fileNew

 Patches binary file using a binary diff.
 Everything is as with <tip>Xdiff.PatchBinary</tip>.
 fileNew receives results.


str s1.getfile(fileOld) s2.getfile(fileDiff) s3

PatchBinary(s1 s2 s3)

s3.setfile(fileNew)

err+ end _error
