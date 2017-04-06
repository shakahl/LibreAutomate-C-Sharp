function'DATE

 Returns date/time in DATE format.


DATE d; SYSTEMTIME st
if(!FileTimeToSystemTime(+&t &st) or !SystemTimeToVariantTime(&st &d.date)) end ERR_FAILED
ret d
