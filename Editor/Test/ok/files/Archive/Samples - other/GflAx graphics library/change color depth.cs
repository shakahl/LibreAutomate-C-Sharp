typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
 g.EnableLZW=TRUE ;;enable for gif
g.LoadBitmap(_s.expandpath("$my pictures$\bird.bmp"))
g.ChangeColorDepth(GflAx.AX_To16Colors 0 0)
g.SaveBitmap(_s.expandpath("$my pictures$\bird2.bmp"))
