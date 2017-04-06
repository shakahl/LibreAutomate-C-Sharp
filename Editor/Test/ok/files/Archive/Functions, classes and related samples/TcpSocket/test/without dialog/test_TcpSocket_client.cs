 Connects to server created by test_TcpSocket_server.

out

#compile "__TcpSocket"
TcpSocket x.ClientConnect("localhost" 5032)

x.Send("request 1")

str s
x.Receive(s 1000)
 x.Receive(s 11)
 x.Receive(s 0 2000)
 x.Receive(s 0)
out F"CLIENT: server response: {s}"

x.Send("request 2")

 x.Receive(s 1000)
x.Receive(s 0)
out F"CLIENT: server response: {s}"
