str sfrom sto
 sfrom="F:\1830_usa_ddk.iso"
 sto="F:\1830_usa_ddk_2.iso"
sfrom="\\GINTARAS\F\1830_usa_ddk.iso" ;;236 MB
sto.expandpath("$desktop$\1830_usa_ddk.iso")

 cop sfrom sto

int canceled
 if(!CopyFileEx(sfrom sto &CFE_Progress 0 &canceled COPY_FILE_RESTARTABLE))
if(!CopyFileEx(sfrom sto 0 0 &canceled COPY_FILE_RESTARTABLE))
	 out canceled
	end _s.dllerror

 CopyFileEx supports resuming, but then it is much slower.
 I tested on 100Mbs network. Speed was (% of network speed):
   cop - 85%.
   CopyFileEx without COPY_FILE_RESTARTABLE - 85%.
   CopyFileEx with COPY_FILE_RESTARTABLE - 33%.

 CopyFileEx itself does not show a progress dialog.
 If eg network connection is lost while copying, CopyFileEx
 fails and leaves partially copied file. When calling next time,
 it resumes copying.
