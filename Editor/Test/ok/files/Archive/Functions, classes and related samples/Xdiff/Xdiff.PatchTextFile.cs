function $fileOld $fileDiff $fileNew [flags] [str&sRejected] ;;flags: 1 reverse, 0x100 ignore whitespace

 Patches text file using a unified diff.
 Everything is as with <tip>Xdiff.PatchText</tip>.
 fileNew receives results.


str s1.getfile(fileOld) s2.getfile(fileDiff) s3

 tested: ignores header

PatchText(s1 s2 s3 flags sRejected)

s3.setfile(fileNew)

err+ end _error
