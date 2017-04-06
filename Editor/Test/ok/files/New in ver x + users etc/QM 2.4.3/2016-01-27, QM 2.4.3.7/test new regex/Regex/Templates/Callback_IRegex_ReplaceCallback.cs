function# cbParam str&match REGEXREPLACECB&x

 Callback function for IRegex.ReplaceCallbackX() functions.
 Called for each match to provide replacements.
 To create the callback function, use menu File -> New -> Templates -> Callback -> Callback_IRegex_ReplaceCallback.

 cbParam - cbParam passed to ReplaceX(). Can be declared as SOMETYPE&r, and then a SOMETYPE variable address passed to ReplaceRX().
 match - contains the match string. This function can modify it; it will be the replacement.
 x - contains some info about the match:
   type REGEXREPLACECB IRegex'x $s str't number
   x - IRegex variable. You can call x.x.GetX() or x.x.offsets() to get current match and submatches.
   s - ReplaceX() parameter s (unchanged).
   t - a temporary string that is being created from s and replacements. Finally s will become = t. Contains the part of s from the beginning to the current match, with previous replacements.
   number - 1-based index of current match.

 Return:
   0 - match is the replacement.
   >0 - let the replacement be none. The callback function itself can append a replacement to x.t.
   -1 - stop and don't include this replacement.
   -2 - stop and include this replacement.
   <-2 - generate error with this error code. Can be one of PCRE2_ERROR_x error constants, or <-100 if don't want to use them.

 EXAMPLE
 str s="one <u>two</u> three <u>four</u>"
 IRegex x=CreateRegex("<u>(.+?)</u>")
 x.ReplaceCallback(s &sub.Callback_IRegex_ReplaceCallback)
 out s
 
 #sub Callback_IRegex_ReplaceCallback ;;callback function
 function# cbParam str&match REGEXREPLACECB&x
 x.x.GetStr(1 match)
 match.ucase
