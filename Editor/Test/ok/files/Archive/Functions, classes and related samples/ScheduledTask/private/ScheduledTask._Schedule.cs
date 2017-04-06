function'ITaskTrigger TASK_TRIGGER&t triggerType $_time

 Creates ITaskTrigger and gets initial TASK_TRIGGER.

ITaskTrigger trigger; m_task.CreateTrigger(+&_i &trigger)
t.cbTriggerSize=sizeof(t); trigger.GetTrigger(&t)
t.rgFlags=0
t.TriggerType=triggerType

DATE d; SYSTEMTIME st
if !empty(_time)
	d=_time; err ret
	d.tosystemtime(st)
	t.wStartHour=st.wHour
	t.wStartMinute=st.wMinute

ret trigger

err+
