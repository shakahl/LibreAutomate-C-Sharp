 /
function $name [^timeoutS]

 Call this from a file-triggered macro to join multiple events for a file to single event.
 Waits for next event for this file (name) max timeoutS seconds. If there is new event, ends this thread.
 Ignores event type. Should be used only with triggers where event types are 'created' and/or 'modified'.
 Default timeoutMS is 1 s.

 EXAMPLE
 function event $name [$newname]
  event: 1 added, 2 removed, 4 renamed, 8 modified
 FileTriggerSkipDuplicateEvent name
 out F"{event} {name} {newname}"


if(!timeoutS) timeoutS=1
_s.encrypt(2|8)
__Handle ev=CreateEvent(0 0 0 F"QM FTSDE {_s}") ;;create or open event for this file
if(GetLastError=ERROR_ALREADY_EXISTS) SetEvent ev ;;if other thread already created event and waiting, set event, let it end
wait timeoutS H ev
err ret ;;timeout. Were no events for this file for timeoutS seconds.
end ;;other thread have set event for this file. End this thread, because other thread will process it.
