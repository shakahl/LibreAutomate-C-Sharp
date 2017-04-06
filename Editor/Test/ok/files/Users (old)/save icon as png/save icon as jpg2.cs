int hi=GetIcon("shell32.dll,16" 1)

int dc0=GetDC(0)
int bm=CreateCompatibleBitmap(dc0 32 32)
ReleaseDC(0 dc0)
int dc=CreateCompatibleDC(0)
int oldbm=SelectObject(dc bm)

RECT r; r.right=32; r.bottom=32
FillRect dc &r GetStockObject(0) ;;WHITE_BRUSH
DrawIconEx dc 0 0 hi 32 32 0 0 3

SelectObject dc oldbm
DeleteDC dc

SaveBitmap bm "$desktop$\test.png"
