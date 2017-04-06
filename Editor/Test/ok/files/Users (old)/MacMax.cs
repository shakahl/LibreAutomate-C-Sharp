int windowLeft windowTop windowWidth
int theWindowHdl = win()

GetWinXY theWindowHdl &windowLeft &windowTop &windowWidth
int maxRows

  maxRows is the total number of rows of text which can fit on the screen. Ideally you’d
  want to calculate it on the fly from the monitor height, taskbar height, font height, etc.
  Instead, I’ve just entered the values which are correct for my screen.
  !!! You’ll want to put your own value, here:

if (windowLeft + (windowWidth/2) < 0)
	maxRows = 54
else
	maxRows = 66

maxRows=39 ;;my
if(IsZoomed(theWindowHdl)) res theWindowHdl ;;my

int detailsPanel = child("" "SysHeader32" theWindowHdl 0x401)
if (!detailsPanel)
	men 28715 ;;Make sure it’s List view

Acc thisItem = acc("" "LISTITEM" theWindowHdl "SysListView32" "" 0x1000)
int x y cx cy
int numItems = 0
  These values are the minimum allowable size of a window.
  You can change this to your own taste.
if(!thisItem.a)
	cx = 240
	cy = 17
else
	rep
		numItems += 1
		thisItem.Navigate("next")
	err+
numItems += 1
thisItem.Location(x y cx cy)
err
	ret

int endWidth endHeight
  Is it a details window, or a list window
if (detailsPanel)
	if (numItems > maxRows)
		numItems = maxRows
		  This is the width of the scrollbar for my choice of theme.
		  !!! You may need to change it slightly for your own theme.
		cx += 14
	endWidth = cx+12
	  This is the extra height (title bar + menu bar + details contols height)
	  !!! You’ll probably want to change it for your own fonts and theme
	endHeight = cy*numItems+75
else
	int numColumns = 1+ (numItems/maxRows)
	int itemsPerColumn = (numItems+numColumns-1) / numColumns
	  This logic avoids a really tall, skinny window
	if ((cy*itemsPerColumn)/iif((cx*numColumns>180) (cx*numColumns) 180)>2)
		numColumns +=1
		itemsPerColumn = (numItems+numColumns-1) / numColumns
	  This sets a minimum width for the window, again
	if (cx*numColumns < 180)
		cx = 180/numColumns
	endWidth = ((cx*numColumns)+12-iif((numColumns=1) 6 0))
	  These lines set a maximum width for the window.
	  !!! You’ll want to change this to match your own screen(s)
	if (windowLeft + (windowWidth/2) < 0) ;; If it's on the left screen
		if (endWidth>1278)
			endWidth = 1278
	else
		if (endWidth > 1598)
			endWidth = 1598
	  Again, this is the extra height (title bar + menu bar)
	  !!! You’ll probably want to change it for your own fonts and theme
	endHeight = (cy*itemsPerColumn)+69

  Now we make sure the window isn’t too tall
  The constants here are from the height of the monitor(s).
  !!! You’ll want to change these to match the height of your own screen(s)
if (windowLeft + (windowWidth/2) < 0) ;; If it's on the left screen
	if (windowTop + endHeight > 1168) ;; Need to adjust vertical
		windowTop = 1169 - endHeight
	if (windowLeft + endWidth > 0) ;; Need to adjust horizontal
		windowLeft = 0 - endWidth
else ;; on the right screen
	if (windowTop + endHeight > 1198) ;; Need to adjust vertical
		windowTop = 1197 - endHeight
	if (windowLeft + endWidth > 1599) ;; Need to adjust horizontal
		windowLeft = 1599 - endWidth
MoveWindow theWindowHdl windowLeft windowTop endWidth endHeight 1
