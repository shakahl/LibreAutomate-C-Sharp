dll ntdll #NtSetTimerResolution desired set *current
dll ntdll #NtQueryTimerResolution *maxi *mini *cur

int mini maxi cur
if(NtQueryTimerResolution(&maxi &mini &cur)) ret
out F"{mini} {maxi} {cur}"; ret

if(NtSetTimerResolution(10000000/64 1 &_i)) ret ;;default, 156250 (15.625 ms)
 if(NtSetTimerResolution(5000 1 &_i)) ret ;;minimal (0.5 ms)
 if(NtSetTimerResolution(20000 1 &_i)) ret
out _i

 NtSetTimerResolution sometimes can make bigger than current, sometimes can't. To restore normal when 1 ms, exit VS.
