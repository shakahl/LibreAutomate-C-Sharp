dll mscoree #GetFileVersion @*szFilename @*szBuffer cchBuffer *dwLength

BSTR b.alloc(100)
out GetFileVersion(@"q:\app\qmnet.dll" b 100 &_i)
 out GetFileVersion(@"Q:\Downloads\cs-script\Lib\CSScriptLibrary.dll" b 100 &_i)
 out GetFileVersion(@"Q:\Downloads\cs-script\Lib\Bin\NET 3.5\CSScriptLibrary.v3.5.dll" b 100 &_i)
out b
