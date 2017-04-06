ARRAY(str) a.create(1 3)
a[0 0]="A"; a[0 1]="''B''"; a[0 2]="C[]D[]E"
ArrayToCsvFile_str2d a "$temp qm$\2.xml"


 ...


ARRAY(str) b
ArrayFromCsvFile_str2d b "$temp qm$\2.xml"
out b

