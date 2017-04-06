function! __AccIdentity&ai

if(!m_b or !ai.m_b) end ERR_INIT
ret m_len=ai.m_len and !memcmp(m_b ai.m_b m_len)
