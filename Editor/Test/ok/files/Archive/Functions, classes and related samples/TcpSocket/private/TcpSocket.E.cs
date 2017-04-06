function [flags] ;;flags: 1 call Close

_s.dllerror("Socket error: " "" WSAGetLastError)
if(flags&1) Close
end _s 2
