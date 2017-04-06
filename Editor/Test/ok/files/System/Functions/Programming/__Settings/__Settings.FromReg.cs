function!

 Gets settings from registry, and replaces default values added by Init.
 Returns 1 if settings exist in registry, 0 if not.

 Errors: ERR_INIT (registry value name not set).


if(!m_rName.len) end ERR_INIT
if(!rget(_s m_rName m_rKey)) ret
FromString(_s)
ret 1
