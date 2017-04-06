function DATE'd

 Initializes this variable from a variable of type DATE.


SYSTEMTIME st
if(!VariantTimeToSystemTime(d.date &st) or !SystemTimeToFileTime(&st +&t)) end ERR_BADARG
