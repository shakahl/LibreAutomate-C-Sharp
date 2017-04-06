out "min"
 out SendMessage(win(mouse) WM_NCHITTEST 0 ym<<16|xm)
 out DefWindowProc(win(mouse) WM_NCHITTEST 0 ym<<16|xm)
