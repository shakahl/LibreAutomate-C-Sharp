 Redraws the active window.

int w1=win ;;get handle of active window
RedrawWindow(w1 0 0 RDW_INVALIDATE|RDW_ERASE|RDW_ALLCHILDREN|RDW_FRAME)
