str s
str ss=
 SeAssignPrimaryTokenPrivilege
 SeAuditPrivilege
 SeBackupPrivilege
 SeChangeNotifyPrivilege
 SeCreatePagefilePrivilege
 SeCreatePermanentPrivilege
 SeCreateTokenPrivilege
 SeDebugPrivilege
 SeEnableDelegationPrivilege
 SeIncreaseQuotaPrivilege
 SeIncreaseBasePriorityPrivilege
 SeLoadDriverPrivilege
 SeLockMemoryPrivilege
 SeMachineAccountPrivilege
 SeManageVolumePrivilege
 SeProfileSingleProcessPrivilege
 SeRemoteShutdownPrivilege
 SeRestorePrivilege
 SeSecurityPrivilege
 SeShutdownPrivilege
 SeSyncAgentPrivilege
 SeSystemtimePrivilege
 SeSystemEnvironmentPrivilege
 SeSystemProfilePrivilege
 SeTakeOwnershipPrivilege
 SeTcbPrivilege
 SeUndockPrivilege
 SeUnsolicitedInputPrivilege
foreach s ss
	out "%i %s" SetPrivilege(s) s
