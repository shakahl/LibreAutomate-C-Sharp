function!

 Saves settings in registry.
 Returns: 1 success, 0 failed.

 Errors: ERR_INIT (registry value name not set).


if(!m_rName.len) end ERR_INIT
ToString(_s)
ret 0!rset(_s m_rName m_rKey)
