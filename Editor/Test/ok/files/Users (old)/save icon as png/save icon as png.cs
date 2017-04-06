int hi=GetIcon("shell32.dll,16" 1)

typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
 g.EnableLZW=TRUE ;;enable for gif
g.NewBitmap(32 32 0xffffff)
stdole.IPicture p=g.GetPicture

int dc=CreateCompatibleDC(0)
int oldbm=SelectObject(dc p.Handle)
DrawIconEx dc 0 0 hi 32 32 0 0 3
SelectObject dc oldbm
DeleteDC dc

g.SetPicture(p)
g.SaveFormatName="png"
g.SaveBitmap(_s.expandpath("$desktop$\test.png"))
