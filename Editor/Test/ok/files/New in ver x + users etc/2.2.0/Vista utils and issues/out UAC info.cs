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

int hToken elevationType uiAccess isElevated dwSize; 

OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &hToken);

GetTokenInformation(hToken, TokenElevationType, &elevationType, 4, &dwSize); 
GetTokenInformation(hToken, TokenUIAccess, &uiAccess, 4, &dwSize); 
GetTokenInformation(hToken, TokenElevation, &isElevated, 4, &dwSize); 

out elevationType
out uiAccess
out isElevated
CloseHandle(hToken);
