function# $receivingfunction str&data !_control [flags] ;;flags: 1 no msgbox

 Sends data to computer, to receivingfunction, which must return a nonzero value to continue or 0 to cancel.
 Returns receivingfunction's return value, or 0 if failed.
 Modifies data.


data-"ns  "; data[2]=_control ;;add signature and _control

int r=net(m_ip m_password receivingfunction _s data)
if(r)
	if(flags&1) ret
	
	str se=
 -
 Computer not found. 
 Could not connect. For example, the remote computer is not running, or not connected to the network or Internet, or QM is not running, or QM does not allow to run macros from other computers (Options -> Network -> Allow ... is unchecked on remote computer), or the remote computer is on the Internet and uses a firewall, or different port is specified, or the port is in use, or problems with network connection (try to disable/enable or repair).  
 Password incorrect, or, if password is "", this macro is not allowed to run without a password. 
 Macro not found. 
 Could not start the macro. For example, it contains errors, or other macro is running. 
 Could not send arguments. The macro did not run. 
 Could not retrieve the return value. The macro ran successfully. 
 Run-time error in the macro. 
	se.getl(se r)
	mes "Could not send data to %s.[]Error %i:[]%s" "" "!" m_computer r se
	if(r=3)
		mes "Make sure that password is correct."
		mac "NS_Setup"
else
	r=val(_s)
	if(r) ret r
	if(flags&1) ret
	mes "Could not send data to %s." "" "!" m_computer
