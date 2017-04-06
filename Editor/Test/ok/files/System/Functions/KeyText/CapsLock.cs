function# [on] ;;on: 0 off, 1 on, 2 press without testing.

 Toggles CAPS LOCK key.
 Returns 1 if it was toggled on before, 0 if was off.

 EXAMPLE
 int wasOn=CapsLock(0) ;;must be off
 key "Password"
 if(wasOn) key K ;;restore


sel on
	case 0: ifk(K 1) key K; ret 1
	case 1: ifk(K 1) ret 1; else key K
	case 2: key K
