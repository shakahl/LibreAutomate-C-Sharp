 To test TCP server and client, at first run this function.
 It creates TCP server thread that runs until you end this thread.
 Then run test_TcpSocket_client, one or more times.

if(getopt(nthreads)>1) ret ;;allow single instance
AddTrayIcon "cut.ico" "test_TcpSocket_server[]Ctrl+click to end."

#compile "__TcpSocket"
TcpSocket x.ServerStart(5032 &sub.OnClientConnected)


#sub OnClientConnected
function TcpSocket&client $clientIp param !*reserved

 This function is called in server side, when a client connects.
 This function runs in separate thread for each client connection.

out F"SERVER: client connected: {clientIp}"

str s
client.Receive(s 1000)
out F"SERVER: client request: {s}"

client.Send("response 1")

client.Receive(s 1000)
out F"SERVER: client request: {s}"

client.Send("response 2")


#sub OnClickTrayIcon
out 1
