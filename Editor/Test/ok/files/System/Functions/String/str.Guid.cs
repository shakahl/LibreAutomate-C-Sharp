
 Creates a new GUID and coverts to string like {4D2C57C4-4CB8-4EEA-BE1A-2031EB87026F}.

 REMARKS
 Calls <google>UuidCreate</google> and returns its return value, which is usually 0 (success).

 See also: <str.RandomString>.


GUID g
int r=CoCreateGuid(&g)
FromGUID(g)
ret r
