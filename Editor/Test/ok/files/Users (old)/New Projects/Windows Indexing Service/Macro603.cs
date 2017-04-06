typelib CIODMLib {3BC4F393-652A-11D1-B4D4-00C04FC2DB8D} 1.0
typelib Cisso {4E469DD1-2B6F-11D0-BFBC-0020F8008024} 1.0 0x409

 CIODMLib.AdminIndexServer a._create
 out a.IsRunning
 a.FindFirstCatalog
  out a.FindNextCatalog
 IDispatch d=a.GetCatalog
 CIODMLib.CatAdm c=d
 out c.CatalogLocation
  a.AddCatalog

CIODMLib.AdminIndexServer is._create
CIODMLib.CatAdm c=is.GetCatalogByName("HtmlHelp")
out c.CatalogLocation
out c.FilteredDocumentCount
