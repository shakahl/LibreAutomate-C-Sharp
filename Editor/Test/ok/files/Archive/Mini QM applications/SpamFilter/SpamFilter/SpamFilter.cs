if(getopt(nthreads)>1) ret

#compile "rasapi"
def SF_DIR "$my qm$\SpamFilter"
type SFOPTIONS @flags @period @period0 nlines ~accounts ~accountsr ~sound ~mailapp ~ff
type SFMESSAGE MailBee.Message'm ~uid ~comm size flags
type SFDELETED ~sf DATE'd

SFOPTIONS o
ARRAY(SFMESSAGE) a
Tray tray
int checking timer ictimer iserr quit ev
Pop3Mail p

p.SetSaveFolder(SF_DIR); err
p.p.CodepageMode=1

ev=CreateEvent(0 0 0 0)
RasConnectionNotification(-1 ev RASCN_Connection)

ShowDialog("SF_DlgMain" &SF_DlgMain 0 0 1 0 WS_VISIBLE|DS_SETFOREGROUND)
MessageLoop

CloseHandle ev

 BEGIN PROJECT
 main_function  SpamFilter
 exe_file  $my qm$\SpamFilter.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {9A35C0B1-78E7-4136-BAB8-28D9A245E86B}
 END PROJECT
