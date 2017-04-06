function event data

 events:
def PBT_APMQUERYSUSPEND             0x0000
def PBT_APMQUERYSTANDBY             0x0001
def PBT_APMQUERYSUSPENDFAILED       0x0002
def PBT_APMQUERYSTANDBYFAILED       0x0003
def PBT_APMSUSPEND                  0x0004
def PBT_APMSTANDBY                  0x0005
def PBT_APMRESUMECRITICAL           0x0006
def PBT_APMRESUMESUSPEND            0x0007
def PBT_APMRESUMESTANDBY            0x0008
def PBTF_APMRESUMEFROMFAILURE       0x00000001
def PBT_APMBATTERYLOW               0x0009
def PBT_APMPOWERSTATUSCHANGE        0x000A
def PBT_APMOEMEVENT                 0x000B
def PBT_APMRESUMEAUTOMATIC          0x0012

 bee
 out "%i %i" event data

sel event
	case PBT_APMRESUMEAUTOMATIC ;;after standby or hibernate
	2
	 mac "PM_AfterStandbyHibernate"
	mac "detect when Windows fully booted"
	
	 here you can add more cases to intercept other power management events, like
	 case PBT_APMQUERYSUSPEND ...

