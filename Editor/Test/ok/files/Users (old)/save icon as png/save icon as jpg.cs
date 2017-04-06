int hi=GetIcon("shell32.dll,16" 1)

type PICTDESC cbSizeOfStruct picType hbitmap hpal [8]hmeta xExt yExt [8]hicon [8]hemf
dll olepro32 #OleCreatePictureIndirect PICTDESC*pPictDesc GUID*riid fOwn !*ppvObj

stdole.IPicture p
PICTDESC pd.cbSizeOfStruct=sizeof(PICTDESC)
pd.picType=3
pd.hicon=hi
if(OleCreatePictureIndirect(&pd uuidof(stdole.IPicture) 1 &p)) end "failed"
stdole.SavePicture(+p _s.expandpath("$desktop$\test.ico"))

 using OleCreatePictureIndirect to create IPicture from HICON directly creates distorted picture
