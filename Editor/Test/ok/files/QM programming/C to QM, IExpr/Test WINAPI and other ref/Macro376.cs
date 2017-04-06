 TEST7
 TEST8
 Koiu
 Koiu2
 Koiu3
 Koiu4
 Koiu5

interface# IFolderFilter :IUnknown
	ShouldShow(IShellFolder'psf ITEMIDLIST*pidlFolder ITEMIDLIST*pidlItem)
	GetEnumFlags(IShellFolder'psf ITEMIDLIST*pidlFolder *phwnd *pgrfFlags)
	{9CC22886-DC8E-11d2-B1D0-00C04F8EEB3E}

IFolderFilter f
 f.ShouldShow
