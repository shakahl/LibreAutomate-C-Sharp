out
str s="one <u>two</u> three"
IRegex x=CreateRegex("<u>(?C1)(.+?)(?C2)</u>")
x.SetCallout(&sub.Callback_IRegex_callout 8)
x.Match(s)

#sub Callback_IRegex_callout ;;callback function
function[c]# pcre2_callout_block*c cbParam
 out cbParam
out F"callout {c.callout_number} at offset {c.current_position}"
