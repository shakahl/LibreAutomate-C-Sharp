ARRAY(str) a.create(3)
a[0]="A"; a[1]="''B''"; a[2]="C[]D[]E"
ArrayToCsvFile_str1d a "$temp qm$\1.xml"


 ...


ARRAY(str) b
ArrayFromCsvFile_str1d b "$temp qm$\1.xml"
out b

