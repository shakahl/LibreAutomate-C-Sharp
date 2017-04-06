 \
function h1 h2
 int r=SleepEx(2000 1)
 out F"SleepEx returned {r}"
int r=WaitForMultipleObjectsEx(2 &h1 1 4000 1)
out F"WaitForMultipleObjectsEx returned {r}"
1
out "ret Function258"
