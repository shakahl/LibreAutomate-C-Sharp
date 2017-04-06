 /exe
int g_speak=0
int BUFFER_SIZE=1024
str g_szPipeName="\\.\Pipe\MyNamedPipe"

//Connect to the server pipe using CreateFile()
int hPipe ;;server fails if we close handle
if(!hPipe) hPipe = CreateFile(g_szPipeName GENERIC_READ|GENERIC_WRITE 0 0 OPEN_EXISTING 0 0)

if (INVALID_HANDLE_VALUE == hPipe) 
	out("client: Error occurred while connecting to the server: %d", GetLastError()); 
	ret

int i;
WakeCPU();
PF;
for i 0 5
	sub.PipeClient(hPipe);
	PN;
PO;
     
CloseHandle(hPipe);


#sub PipeClient v
function hPipe

str szBuffer.all(BUFFER_SIZE)
memcpy(szBuffer, "Hello", 6);

int cbBytes;

//Send the message to server
int bResult = WriteFile(hPipe szBuffer len(szBuffer)+1 &cbBytes 0)
if ( (!bResult) || (len(szBuffer)+1 != cbBytes))
	out("client: Error occurred while writing to the server: %d", GetLastError()); 
	CloseHandle(hPipe);
	ret;  ;;Error
else
	if(g_speak) out("client: WriteFile() was successful.");

//Read server response
bResult = ReadFile(hPipe szBuffer BUFFER_SIZE &cbBytes 0)

if ( (!bResult) || (0 == cbBytes)) 
	out("client: Error occurred while reading from the server: %d", GetLastError()); 
	CloseHandle(hPipe);
	ret;  ;;Error

if(g_speak) out("client: Server sent the following message: %s", szBuffer);

 BEGIN PROJECT
 main_function  Macro2751
 exe_file  $my qm$\Macro2751.qmm
 flags  6
 guid  {F50EBC0A-7065-4577-8CC2-84A849B45F22}
 END PROJECT
