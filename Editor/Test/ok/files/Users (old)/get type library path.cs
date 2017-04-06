 typelib MSHTML {3050F1C5-98B5-11CF-BB82-00AA00BDCE0B} 4.0 0 1

dll oleaut32 #QueryPathOfRegTypeLib GUID*guid @wVerMajor @wVerMinor lcid BSTR*lpbstrPathName
dll ole32 IIDFromString @*lpsz GUID*lpiid

str guidstr="{3050F1C5-98B5-11CF-BB82-00AA00BDCE0B}"
GUID guidbin
guidstr.unicode
IIDFromString(+guidstr &guidbin)
BSTR s
QueryPathOfRegTypeLib(&guidbin 4 0 0 &s)
out s
