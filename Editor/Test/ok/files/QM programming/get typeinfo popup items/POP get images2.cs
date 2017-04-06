 Converts typeinfo popup icons to gifs, and saves in $qm$\web\images\ti and $qm$\htmlhelp\image\ti.

out

str fdest.expandpath("$qm$\web\images\ti")
mkdir fdest
str fdest2.expandpath("$qm$\htmlhelp\image\ti")
mkdir fdest2
mkdir "$temp$\qm"
str tempbmp.expandpath("$temp$\qm\qmbmp.bmp")

__MemBmp mb.Create(16 16)
__GdiHandle brush=CreateSolidBrush(0xffffff)
RECT r.right=16; r.bottom=16

typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
g.EnableLZW=TRUE ;;enable for gif

int i
Dir d
foreach(d "$qm$\icons\ti\*.ico" FE_Dir)
	str sPath=d.FileName(1)
	str fn.getfilename(sPath)
	
	FillRect mb.dc &r brush
	int ic=GetFileIcon(sPath)
	DrawIconEx(mb.dc 0 0 ic 16 16 0 0 DI_NORMAL)
	if(!SaveBitmap(mb.bm tempbmp)) end "error"
	
	g.LoadBitmap(tempbmp)
	g.ChangeColorDepth(GflAx.AX_To16Colors 0 0)
	g.SaveFormatName="gif"
	g.SaveBitmap(_s.from(fdest "\" fn ".gif"))
	g.SaveBitmap(_s.from(fdest2 "\" fn ".gif"))
	

 uses temp bmp etc because gfl makes black background if loads ico
