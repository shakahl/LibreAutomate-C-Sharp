def TokenElevationTypeDefault 1
def TokenElevationTypeFull 2
def TokenElevationTypeLimited 3

def TokenUser 1
def TokenGroups 2
def TokenPrivileges 3
def TokenOwner 4
def TokenPrimaryGroup 5
def TokenDefaultDacl 6
def TokenSource 7
def TokenType 8
def TokenImpersonationLevel 9
def TokenStatistics 10
def TokenRestrictedSids 11
def TokenSessionId 12
def TokenGroupsAndPrivileges 13
def TokenSessionReference 14
def TokenSandBoxInert 15
def TokenAuditPolicy 16
def TokenOrigin 17
def TokenElevationType 18
def TokenLinkedToken 19
def TokenElevation 20
def TokenHasRestrictions 21
def TokenAccessInformation 22
def TokenVirtualizationAllowed 23
def TokenVirtualizationEnabled 24
def TokenIntegrityLevel 25
def TokenUIAccess 26
def TokenMandatoryPolicy 27
def TokenLogonSid 28
def MaxTokenInfoClass 29

int hToken elevationType uiAccess isElevated IL MP dwSize; 

int w=win("Quick")
int pid
GetWindowThreadProcessId(w &pid)

__Handle hp ht
hp=OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, 0, pid);
if(!OpenProcessToken(hp, TOKEN_QUERY, &ht)) ret;

out
out GetTokenInformation(ht, TokenElevationType, &elevationType, 4, &dwSize); 
out GetTokenInformation(ht, TokenUIAccess, &uiAccess, 4, &dwSize); 
out GetTokenInformation(ht, TokenElevation, &isElevated, 4, &dwSize); 
out GetTokenInformation(ht, TokenMandatoryPolicy, &MP, 4, &dwSize);

type TOKEN_MANDATORY_LABEL :SID_AND_ATTRIBUTES
TOKEN_MANDATORY_LABEL* til
if(!GetTokenInformation(ht, TokenIntegrityLevel, til, 0, &dwSize)) out _s.dllerror; 


out "----"
out elevationType
out uiAccess
out isElevated
out IL
out MP


