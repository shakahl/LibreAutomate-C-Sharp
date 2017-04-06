function ExcelSheet&es [offFlags] ;;offFlags: 1 alerts, 2 screen updating

 Temporarily changes some Excel features. Auto restores in destructor.


m_a=es.ws.Application; err ret
if(offFlags&1) AlertsOff
if(offFlags&2) UpdatingOff
