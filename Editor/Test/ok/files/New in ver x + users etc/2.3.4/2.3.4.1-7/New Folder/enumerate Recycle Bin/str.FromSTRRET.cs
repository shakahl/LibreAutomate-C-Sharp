function$ STRRET&sr [ITEMIDLIST*pidl]

word* w
StrRetToStrW(&sr pidl &w)
ansi(w)
CoTaskMemFree w
ret this
