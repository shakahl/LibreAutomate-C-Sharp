When there are many files (one for each script), loading all is very slow, especially when slow disk, slow AV, cold.
Eg can be 35s (11693 files, 10.8 MB, HDD, Windows Defender, cold).
Therefore for Find we use a cache: write text of all scripts to several files eg about 1 MB each.
