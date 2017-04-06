SetAttr "$desktop$\test.txt" FILE_ATTRIBUTE_READONLY|FILE_ATTRIBUTE_ARCHIVE

int a=GetAttr("$desktop$\test.txt")
out "0x%X" a
