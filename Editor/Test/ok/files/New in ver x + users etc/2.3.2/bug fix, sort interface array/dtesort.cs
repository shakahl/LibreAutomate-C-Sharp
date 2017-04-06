function# param EnvDTE.Window&a EnvDTE.Window&b

str s1 s2
s1=a.Document.Name
s2=b.Document.Name
ret StrCompare(s1 s2)
