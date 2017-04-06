Reset trial period on new higher QM version.

Allow 30 days (simple) + 30 days of real usage. Max 6 months.
   A real usage day is when started >= 10 threads / day.

Check expiration every 4-th day. Then 3 from 4 users cannot discover protection with regmon etc.

When expired, let QM still works every 2-nd day.

__________________________

Trial data is stored in registry.
It is also stored in online DB, when connection available. If registry data deleted, restores from the DB. Also useful to get usage stats.
Online DB table has 3 columns: 1. CRC of hard disk serial and $desktop$ path (includes user name). 2. QM version. 3. Trial data (used days etc).


__________________________

REJECTED

After some time start to connect to QM online database occasionally. Look for disabled regcodes. Detect shared regcodes.
   It's dangerous. May be identified as a spyware. Keyboard hooks, etc...

__________________________

IMPLEMENTATION

At startup run __Decrypt (like now):
	Get and verify regcode.
	If regcode not found or invalid:
		Get usage data (number days used, QM version, etc) from registry: HKCU\Software\GinDi:QMA.
			If current QM version is higher, assume not found.
		Get usage data from alt file stream: $temp$:sf_18p359. If failed, from file $temp$\sf_18p359.tmp.
			If current QM version is higher, assume not found.
		If both found but different, use one with bigger num days.
		If not found, assume this is first time. Set num_days_used=0.
		If num_days_used>30, show reminder.
		If num_days_used>45, disable QM.

If in evaluation period:
	On every user thread start, increase counter.
	If counter == 5-15 (random):
		Stop increasing counter etc.
		Write usage data to registry and file stream.

Usage data format:
	type TRIAL qmVersion !nUsedDays @lastUsedDay
	If QMVER>qmVersion, nUsedDays=0.
	Else if today > lastUsedDay, increment nUsedDays.
	