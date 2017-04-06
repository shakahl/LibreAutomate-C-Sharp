 /
function# [int&daysLeft]

 If your product is registered or the evaluation period is not expired, this function returns your-defined nonzero number.
 Otherwise it shows the About dialog and immediately returns 0.

 If daysLeft is used, it receives: -1 if registered, 0 if expired, the number of days left + 1 if evaluating.

 Change the name of this function so that it would be unique.
 Encrypt this function and distribute together with your macros.

 This is your unique data. You must change all these values.
lpstr sm_registry_key_regcode="Software\Company\Product" ;;Change Company\Product to match you company (or name) and product name. This key is not secret. It will be created in HKEY_CURRENT_USER hive. Use the same value in ..._CanRun.
lpstr sm_registry_key_first_run="Software\Company\Product" ;;Choose some place in the registry, HKEY_CURRENT_USER hive, where nobody could find it.
lpstr sm_encryption_key="encryption key" ;;Some unique string. Use the same value in SM_keygen.
int sm_evaluation_period=15 ;;Change this value if you want to use different evaluation period.
int sm_ret=1522539484 ;;Change this number to some unique number. Use that number in protected macros.
lpstr sm_product_name="SM Macros" ;;change this
 Also, change SM_About (in 1 place below) to the name of the renamed function (you have to rename SM_About because it must have a unique name).

 Remaining code does not need to be changed. You make your regcode generation method unique
 by just providing an unique encryption key (the value of the sm_encryption_key variable).

 ______________________________________________

 Try to get previous result to make faster.
int-- t_days_left
if(t_days_left)
	if(&daysLeft) daysLeft=t_days_left
	ret sm_ret
 ______________________________________________

 1. Is the product registered?

str regcode name encryptedname

 Retrieve regcode from registry.
rget regcode "regcode" sm_registry_key_regcode
if(regcode.len<18) goto g1

 Get name.
name.get(regcode 17)
name.escape(8) ;;unescape dangerous characters

 Encrypt the name. Use the same algorithm as is used for regcode generation.
encryptedname.encrypt(9 name sm_encryption_key)
encryptedname.fix(16)

 Compare.
if(!regcode.beg(encryptedname)) goto g1

 Compare c: serial, if any.
if(findrx(name "<\d+>" 0 0 _s)>=0)
	dll kernel32 #GetVolumeInformation $lpRootPathName $lpVolumeNameBuffer nVolumeNameSize *lpVolumeSerialNumber *lpMaximumComponentLength *lpFileSystemFlags $lpFileSystemNameBuffer nFileSystemNameSize
	GetVolumeInformation("c:\" 0 0 &_i 0 0 0 0)
	if(val(_s+1)!=_i&0xffff) mes "This registration code cannot be used on this computer or hard drive. Please contact the author of this product (click Support in the next dialog)." sm_product_name "x"; goto g1

if(&daysLeft) daysLeft=-1
t_days_left=-1
ret sm_ret ;;regitered

 ______________________________________________
 
 2. Is the evaluation period expired?

 g1
DATE date_first date_now.getclock

 Get the first run date.
if(!rget(date_first "" sm_registry_key_first_run))
	 Your product is used first time. Save current date.	
	if(!rset(date_now "" sm_registry_key_first_run))
		out "Failed. To run this macro first time on this computer, log on as an administrator."
		ret
else
	 How long your product is used?
	if(date_now<date_first or date_now>date_first+sm_evaluation_period) goto expired
	sm_evaluation_period-(date_now-date_first)-1

if(&daysLeft) daysLeft=sm_evaluation_period
t_days_left=sm_evaluation_period
ret sm_ret ;;not expired

 expired
wait 0 H mac("SM_About" "" 0 1)
