str s
 if(!GetQmSharedFolderFilePath("\System" s)) end "failed"
 if(!GetQmSharedFolderFilePath(+qmitem("\System") s)) end "failed"
if(!GetQmSharedFolderFilePath("{ACBAA175-3536-4603-9C7D-BF35F590075A}" s)) end "failed"
out s
