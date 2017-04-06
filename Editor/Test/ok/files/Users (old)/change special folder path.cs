dll shell32 [232]#SHSetFolderPathW csidl hToken dwFlags @*pszPath

 show current "$my pictures$" path and remember it
out _s.expandpath("$my pictures$") ;;current
str savymypict.expandpath("$my pictures$")

 create new folder and set it as "$my pictures$"
str f.expandpath("$desktop$\My Pictures2")
mkdir f
SHSetFolderPathW CSIDL_MYPICTURES 0 0 @f
out _s.expandpath("$my pictures$") ;;changed

 restore
SHSetFolderPathW CSIDL_MYPICTURES 0 0 @savymypict
out _s.expandpath("$my pictures$") ;;restored

 CSIDL constants are in MSDN library.
 Also read about SHSetFolderPath there.
