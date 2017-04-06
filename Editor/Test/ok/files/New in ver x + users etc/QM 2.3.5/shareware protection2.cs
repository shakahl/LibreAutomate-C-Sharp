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
   It's dangerous. May be identified as a keylogger. Keyboard hooks, unknown publisher...

__________________________

IMPLEMENTATION

Initially QM is fully enabled.
On every user thread start, increase counter.
If counter == 5-15 (random):
	Stop increasing counter etc.
	Get regcode from registry.
	If found:
		Validate:
		If valid, 
If regcode not found or invalid: