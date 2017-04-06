Services.clsService se._create
BSTR dispName="Quick Macros"
se.DisplayName=dispName
if(!se.ServiceType) mes- "Service not found." "" "x"

str state
sel se.CurrentState
	case Services.Stopped state="stopped"
	case Services.Start_Pending_20 state="start pending"
	case Services.Stop_Pending_20 state="stop pending"
	case Services.Running state="running"
	case Services.continue_pending_20 state="continue pending"
	case Services.Pause_Pending_20 state="pause pending"
	case Services.Paused state="paused"
	case else state="unknown"

int i=list("Start[]Stop[]Pause[]Continue" F"Current state: {state}")
if(!i) ret
if(!IsUserAdmin) mes- "To change service state, QM must be running as admin." "" "x"
sel i
	case 1 se.StartService
	case 2 se.StopService
	case 3 se.PauseService
	case 4 se.ContinueService
