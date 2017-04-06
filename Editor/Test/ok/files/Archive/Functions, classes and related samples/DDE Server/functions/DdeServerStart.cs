 /
function $serverName cbFunc [cbParam]

 Starts DDE server.
 Error if fails.
 DDE server receives DDE actions (Execute, Poke, Request) from applications that can work as DDE clients (send these actions). For example, run a macro when a client requests it. Clients also can be other threads in same process.

 serverName - server name. For example, it can be name of your macro or program.
 cbFunc - address of callback function. See <open>dde_server_callback</open>.
   The function will be called when clients connect, disconnect, ececute, poke or request.
 cbParam - some value to pass to the callback function.

 REMARKS
 A thread can have single DDE server.
 The thread must stay running after it calls this function. It must process messages. For example, use dialog, or message loop, or wait with opt waitmsg.
 The thread must not use QM DDE client functions.
 Supported actions: Execute, Poke, Request.
 Supported data formats: CF_UNICODETEXT, CF_TEXT.
 With Execute action, first data character must be ASCII. It is used to detect data format (CF_UNICODETEXT or CF_TEXT).
 Windows Vista/7 UAC:
   DDE server's process must not have higher integrity level than client processes.
   For example, if DDE server's process runs as Administrator, and client processes run as User, clients cannot connect.
   By default, QM runs as Administrator. Therefore DDE server should run in separate process, as User. Or let clients run as Administrator too.


#compile "____DdeServer"

__DdeServer- __t_ddes
__t_ddes.Start(serverName cbFunc cbParam)

err+ end _error
