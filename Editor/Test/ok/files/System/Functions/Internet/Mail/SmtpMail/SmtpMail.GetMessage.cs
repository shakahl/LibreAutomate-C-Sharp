function index MailBee.Message&m ;;Retrieves a message from the collection.

 Gets a Message object added by AddMessage.

 index - 0-based message index. If <0, gets last message.


if(!m_a.len or index>=m_a.len) end ERR_BADARG
if(index<0) index=m_a.len-1

m=m_a[index].m
