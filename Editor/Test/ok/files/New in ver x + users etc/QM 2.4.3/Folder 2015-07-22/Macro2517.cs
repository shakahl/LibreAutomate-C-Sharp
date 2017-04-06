 dll shlwapi @*StrChrW @*lpStart @wMatch
 dll shlwapi @*StrChrW @*lpStart @wMatch

dll shlwapi [StrCSpnA]#StrCSpn $lpStr $lpSet
dll shlwapi [StrCSpnIA]#StrCSpnI $lpStr $lpSet
dll shlwapi #StrCSpnIW @*lpStr @*lpSet
dll shlwapi #StrCSpnW @*lpStr @*lpSet
dll shlwapi [StrCatBuffA]$StrCatBuff $pszDest $pszSrc cchDestBuffSize
dll shlwapi @*StrCatBuffW @*pszDest @*pszSrc cchDestBuffSize
dll shlwapi #StrCatChainW @*pszDst cchDst ichAt @*pszSrc
dll shlwapi @*StrCatW @*psz1 @*psz2
dll shlwapi @*StrChrNIW @*lpStart @wMatch cchMax
dll shlwapi @*StrChrNW @*lpStart @wMatch cchMax
dll shlwapi [StrCmpCA]#StrCmpC $pszStr1 $pszStr2
dll shlwapi #StrCmpCW @*pszStr1 @*pszStr2
dll shlwapi [StrCmpICA]#StrCmpIC $pszStr1 $pszStr2
dll shlwapi #StrCmpICW @*pszStr1 @*pszStr2
dll shlwapi #StrCmpIW @*psz1 @*psz2
dll shlwapi #StrCmpLogicalW @*psz1 @*psz2
dll shlwapi [StrCmpNCA]#StrCmpNC $lpStr1 $lpStr2 nChar
dll shlwapi #StrCmpNCW @*lpStr1 @*lpStr2 nChar
dll shlwapi [StrCmpNICA]#StrCmpNIC $lpStr1 $lpStr2 nChar
dll shlwapi #StrCmpNICW @*lpStr1 @*lpStr2 nChar
dll shlwapi #StrCmpW @*psz1 @*psz2
dll shlwapi @*StrCpyNW @*psz1 @*psz2 cchMax
dll shlwapi @*StrCpyW @*psz1 @*psz2
dll shlwapi [StrDupA]$StrDup $lpSrch
dll shlwapi @*StrDupW @*lpSrch
dll shlwapi [StrFormatByteSizeA]$StrFormatByteSize dw $pszBuf cchBuf
dll shlwapi [StrFormatByteSize64A]$StrFormatByteSize64 %qdw $pszBuf cchBuf
dll shlwapi $StrFormatByteSize64A %qdw $pszBuf cchBuf
dll shlwapi @*StrFormatByteSizeW %qdw @*pszBuf cchBuf
dll shlwapi [StrFormatKBSizeA]$StrFormatKBSize %qdw $pszBuf cchBuf
dll shlwapi @*StrFormatKBSizeW %qdw @*pszBuf cchBuf
dll shlwapi [StrFromTimeIntervalA]#StrFromTimeInterval $pszOut cchMax dwTimeMS digits
dll shlwapi #StrFromTimeIntervalW @*pwszOut cchMax dwTimeMS digits
dll shlwapi [StrIsIntlEqualA]#StrIsIntlEqual fCaseSens $lpString1 $lpString2 nChar
dll shlwapi #StrIsIntlEqualW fCaseSens @*lpString1 @*lpString2 nChar
dll shlwapi [StrNCatA]$StrNCat $psz1 $psz2 cchMax
dll shlwapi @*StrNCatW @*psz1 @*psz2 cchMax
dll shlwapi [StrPBrkA]$StrPBrk $psz $pszSet
dll shlwapi @*StrPBrkW @*psz @*pszSet
dll shlwapi #StrRetToBSTR STRRET*pstr ITEMIDLIST*pidl BSTR*pbstr
dll shlwapi [StrRetToBufA]#StrRetToBuf STRRET*pstr ITEMIDLIST*pidl $pszBuf cchBuf
dll shlwapi #StrRetToBufW STRRET*pstr ITEMIDLIST*pidl @*pszBuf cchBuf
dll shlwapi [StrRetToStrA]#StrRetToStr STRRET*pstr ITEMIDLIST*pidl $*ppsz
dll shlwapi #StrRetToStrW STRRET*pstr ITEMIDLIST*pidl @**ppsz
dll shlwapi [StrSpnA]#StrSpn $psz $pszSet
dll shlwapi #StrSpnW @*psz @*pszSet
dll shlwapi @*StrStrNIW @*lpFirst @*lpSrch cchMax
dll shlwapi @*StrStrNW @*lpFirst @*lpSrch cchMax
dll shlwapi [StrToIntA]#StrToInt $lpSrc
dll shlwapi [StrToInt64ExA]#StrToInt64Ex $pszString dwFlags %*pllRet
dll shlwapi #StrToInt64ExW @*pszString dwFlags %*pllRet
dll shlwapi [StrToIntExA]#StrToIntEx $pszString dwFlags *piRet
dll shlwapi #StrToIntExW @*pszString dwFlags *piRet
dll shlwapi #StrToIntW @*lpSrc
dll shlwapi [StrTrimA]#StrTrim $psz $pszTrimChars
dll shlwapi #StrTrimW @*psz @*pszTrimChars

dll shlwapi [StrChrA]$StrChr $lpStart @wMatch
dll shlwapi [StrChrIA]$StrChrI $lpStart @wMatch
dll shlwapi @*StrChrIW @*lpStart @wMatch
dll shlwapi @*StrChrW @*lpStart @wMatch
dll shlwapi [StrCmpNA]#StrCmpN $lpStr1 $lpStr2 nChar
dll shlwapi [StrCmpNIA]#StrCmpNI $lpStr1 $lpStr2 nChar
dll shlwapi #StrCmpNIW @*lpStr1 @*lpStr2 nChar
dll shlwapi #StrCmpNW @*lpStr1 @*lpStr2 nChar
dll shlwapi [StrRChrA]$StrRChr $lpStart $lpEnd @wMatch
dll shlwapi [StrRChrIA]$StrRChrI $lpStart $lpEnd @wMatch
dll shlwapi @*StrRChrIW @*lpStart @*lpEnd @wMatch
dll shlwapi @*StrRChrW @*lpStart @*lpEnd @wMatch
dll shlwapi [StrRStrIA]$StrRStrI $lpSource $lpLast $lpSrch
dll shlwapi @*StrRStrIW @*lpSource @*lpLast @*lpSrch
dll shlwapi [StrStrA]$StrStr $lpFirst $lpSrch
dll shlwapi [StrStrIA]$StrStrI $lpFirst $lpSrch
dll shlwapi @*StrStrIW @*lpFirst @*lpSrch
dll shlwapi @*StrStrW @*lpFirst @*lpSrch
