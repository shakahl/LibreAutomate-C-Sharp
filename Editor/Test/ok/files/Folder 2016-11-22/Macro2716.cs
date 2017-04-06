 /exe
mes 1
rep
	WINAPI2.SHSTOCKICONINFO x.cbSize=sizeof(x)
	int img=WINAPI2.SIID_APPLICATION
	 int img=WINAPI2.SIID_DOCNOASSOC
	int hr=WINAPI2.SHGetStockIconInfo(img, WINAPI2.SHGSI_ICONLOCATION &x) ;;always fast
	if(hr) end "failed" 16 hr

 BEGIN PROJECT
 main_function  Macro2716
 exe_file  $my qm$\Macro2716.qmm
 flags  6
 guid  {7143AB7C-76B9-4B63-BA68-879E318EB5F8}
 END PROJECT
