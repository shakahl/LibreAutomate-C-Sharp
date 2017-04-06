 \
function# $setupfile $setupparameters

 //!! must upgrade only if admin is logged on

 This function automatically upgrades QM.
 For example, if you have installed QM on several computers
 in network, you can set this function to run on each computer
 when QM starts (from init2 or init3 function). This would
 automatically upgrade QM on all computers.

 When this function runs, it checks setupfile modification
 time. If it is changed, launches setupfile (except first time).
 You can find available setup parameters in QM Help.

 EXAMPLE
 AutoUpgradeQM "\\server\shareddocs\quickmac.exe" "/SILENT"


str s.expandpath(setupfile)
if(!dir(s)) bee; out "Unable to upgrade QM: the setup file not found."; ret
long t1; rget t1 "setupfiletime"
long& t2=+&_dir.fd.ftLastWriteTime
if(t1=t2) ret 1
rset t2 "setupfiletime"
if(!t1) ret 1

run s setupparameters
shutdown -1
