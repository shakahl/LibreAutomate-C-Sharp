def TB_GETTOOLTIPS (WM_USER + 35)
def TTM_SETDELAYTIME (WM_USER + 3)
def TTDT_INITIAL 3

int h=id(9999 val(_command))
int htt=SendMessage(h TB_GETTOOLTIPS 0 0)

SendMessage(htt TTM_SETDELAYTIME TTDT_INITIAL 10)

