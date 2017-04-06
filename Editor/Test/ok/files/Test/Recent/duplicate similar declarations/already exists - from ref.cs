 out DMLERR_BUSY
 def DMLERR_BUSY 0x4001
  def DMLERR_BUSY 0x04001

 WINAPI.POINTF p
 type POINTF FLOAT'x FLOAT'y
  type POINTF FLOAT'x  FLOAT'y
 POINTF pp
 p.y=1.5
 pp.y=1.5
 out p.y
 out pp.y

 TRF.IFolderFilter f
 interface# IFolderFilter :IUnknown
	 ShouldShow(IShellFolder'psf ITEMIDLIST*pidlFolder ITEMIDLIST*pidlItem)
	 GetEnumFlags(IShellFolder'psf ITEMIDLIST*pidlFolder *phwnd *pgrfFlags)
	 {9CC22886-DC8E-11d2-B1D0-00C04F8EEB3E}

 out &WINAPI.GetThemeInt
 dll uxtheme #GetThemeInt hTheme iPartId iStateId iPropId *piVal
 out &WINAPI.GetThemeInt

 TRF.MSDataGridLib.DataGrid g
 typelib MSDataGridLib {CDE57A40-8B86-11D0-B3C6-00A0C90AEA82} 1.0 3
