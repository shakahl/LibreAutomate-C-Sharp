function cbFunc [cbParam]

 Sets callout callback function that will be called by Match().

 cbFunc - callback function address.
 cbParam - something to pass to the callback function.


if(!_mc) _mc=pcre2_match_context_create(0)
pcre2_set_callout(_mc cbFunc +cbParam)
