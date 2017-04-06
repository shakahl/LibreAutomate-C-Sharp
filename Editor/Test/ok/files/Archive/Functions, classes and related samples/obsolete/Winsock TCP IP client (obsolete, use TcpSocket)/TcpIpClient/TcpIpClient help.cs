 Note: You should instead use TcpSocket class, it's in Archive too. It is newer, and maybe bugs fixed etc.

 Sends and receives data using TCP/IP.
 A variable of TcpIpClient type also can be used with Winsock functions as SOCKET variable.
 When a function fails, it returns 0. To get error code, call WSAGetLastError.

 EXAMPLE

 Connects to http server and downloads file.

out
#compile "__TcpIpClient"
TcpIpClient x

str server="en.wikipedia.org"
str getfile="/wiki/Hypertext_Transfer_Protocol"
str receivedData

out "---- connecting ----"

if(!x.Connect(server 80)) end "failed"

out "---- sending request ----"

str request.format("GET %s HTTP/1.0[]User-Agent: Quick Macros[]Host: %s[][]" getfile server)
if(!x.Send(request)) end "failed"

out "---- receiving ----"

if(!x.Receive(receivedData)) end "failed"

out "---- received: ----"
out receivedData
