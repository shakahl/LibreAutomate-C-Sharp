 win "" "" "" 0 F"callback={&Callback_win_or_child3}"

win "" "" "" 0 F"callback={&sub.Callback}"

#sub Callback
function# hwnd cbParam
outw hwnd
ret 1
