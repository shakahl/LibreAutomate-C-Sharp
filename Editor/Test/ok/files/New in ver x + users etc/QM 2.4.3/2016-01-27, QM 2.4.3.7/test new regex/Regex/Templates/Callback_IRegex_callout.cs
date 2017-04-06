function[c]# pcre2_callout_block*c cbParam

 Callout callback function for IRegex.MatchX() and other functions.
 Called for each reached (?Cn) specified in regular expression.
 To create the callback function, use menu File -> New -> Templates -> Callback -> Callback_IRegex_callout.

 Return:
   0 - continue as normal.
   1 - fail matching at this point, but continue searching.
   <0 - generate error with this error number.

 More info:
 <link>http://www.pcre.org/current/doc/html/pcre2callout.html</link>

 EXAMPLE
 out
 str s="one <u>two</u> three"
 IRegex x=CreateRegex("<u>(?C1)(.+?)(?C2)</u>")
 x.SetCallout(&sub.Callback_IRegex_callout)
 x.Match(s)
 
 #sub Callback_IRegex_callout ;;callback function
 function[c]# pcre2_callout_block*c cbParam
 out F"callout {c.callout_number} at offset {c.current_position}"
