/exe
 out

dll shell32 [660]#FileIconInit fRestoreCache
 out FileIconInit(0)

IImageList il

rep 1
	if(SHGetImageList(SHIL_SMALL IID_IImageList &il)) ret
	out il
	il.GetImageCount(_i); out _i
	int cx cy; il.GetIconSize(&cx &cy); out cx
	
	 if(cx!=16)
		 il.SetIconSize(16 16)
		  FileIconInit(0)


outref il

 BEGIN PROJECT
 main_function  Macro2752
 exe_file  $my qm$\Macro2752.qmm
 flags  6
 guid  {D17063E5-32DA-4AF8-AF45-2626A3E431DA}
 END PROJECT
