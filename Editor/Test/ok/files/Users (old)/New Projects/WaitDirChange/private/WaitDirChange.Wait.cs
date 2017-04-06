function [^nsec]

int fl=FILE_NOTIFY_CHANGE_FILE_NAME|FILE_NOTIFY_CHANGE_DIR_NAME|FILE_NOTIFY_CHANGE_ATTRIBUTES|FILE_NOTIFY_CHANGE_SIZE|FILE_NOTIFY_CHANGE_LAST_WRITE

if(m_h) FindNextChangeNotification(m_h)
else m_h=FindFirstChangeNotification(s m_tree fl); if(!m_h) end ES_FAILED 3

wait nsec H m_h; err end _error 3
