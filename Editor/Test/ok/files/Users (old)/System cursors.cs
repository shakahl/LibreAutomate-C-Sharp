dll user32 #LoadCursorFromFile $lpFileName
dll user32 #SetSystemCursor hcur id
def OCR_NORMAL       32512
def SPI_SETCURSORS   87

 set custom arrow cursor
int hcur=LoadCursorFromFile("C:\WINDOWS\Cursors\arrow_rl.cur")
if(!hcur) ret ;;file not found
SetSystemCursor(hcur OCR_NORMAL)
 wait 10 s
10
 restore system cursors
SystemParametersInfo SPI_SETCURSORS 0 0 0
