 /
function msg wParam !*data [datasize] [cwnd] ;;cwnd is handle to the window passing the data

int BBhwnd = FindWindow("BlackBoxClass", "BlackBox"); if(!BBhwnd) ret

if(getopt(nargs)<4) datasize=-1
str s.fromn(&wParam 4 data datasize)

type COPYDATASTRUCT dwData cbData lpData
COPYDATASTRUCT cds; 
cds.dwData = msg; 
cds.cbData = s.len; 
cds.lpData = s; 
SendMessage(BBhwnd, WM_COPYDATA, cwnd, &cds)
 ___________________________
  define this in init2 function
 def BB_BROADCAST 10901
 ___________________________
  call the function
 SendBB(BB_BROADCAST, 0, "@BBYourBroam"); 
