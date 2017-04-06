 This Ethernet Adapter physical address (MAC) can be used with any computer and any adapter to connect to zirzile.
 
 0018F38BF1C2
 
 I changed it on some computers/adapters for this purpose.
 
 However, use the changed MAC only with adapters that connect to the Internet.
 If you use the same MAC address with adapters that are used to connect computers, connection fails.
 
 To change or restore MAC for an adapter, can be used a free program etherchange (now in USB stick 256MB).
 It sets or restores a registry value. You could do it in regedit but it is quite difficult to find where/what to change.


 To view MAC:
RunConsole2 "ipconfig.exe /all"
 RunConsole2 "ipconfig.exe /renew"

 ipconfig usage:

     ipconfig [/allcompartments] [/? | /all | 
                                  /renew [adapter] | /release [adapter] |
                                  /renew6 [adapter] | /release6 [adapter] |
                                  /flushdns | /displaydns | /registerdns |
                                  /showclassid adapter |
                                  /setclassid adapter [classid] |
                                  /showclassid6 adapter |
                                  /setclassid6 adapter [classid] ]
 
 where
     adapter             Connection name 
                        (wildcard characters * and ? allowed, see examples)
 
     Options:
        /?               Display this help message
        /all             Display full configuration information.
        /release         Release the IPv4 address for the specified adapter.
        /release6        Release the IPv6 address for the specified adapter.
        /renew           Renew the IPv4 address for the specified adapter.
        /renew6          Renew the IPv6 address for the specified adapter.
        /flushdns        Purges the DNS Resolver cache.
        /registerdns     Refreshes all DHCP leases and re-registers DNS names
        /displaydns      Display the contents of the DNS Resolver Cache.
        /showclassid     Displays all the dhcp class IDs allowed for adapter.
        /setclassid      Modifies the dhcp class id.  
        /showclassid6    Displays all the IPv6 DHCP class IDs allowed for adapter.
        /setclassid6     Modifies the IPv6 DHCP class id.
 
 
 The default is to display only the IP address, subnet mask and
 default gateway for each adapter bound to TCP/IP.
 
 For Release and Renew, if no adapter name is specified, then the IP address
 leases for all adapters bound to TCP/IP will be released or renewed.
 
 For Setclassid and Setclassid6, if no ClassId is specified, then the ClassId is removed.
 
 Examples:
     > ipconfig                       ... Show information
     > ipconfig /all                  ... Show detailed information
     > ipconfig /renew                ... renew all adapters
     > ipconfig /renew EL*            ... renew any connection that has its 
                                          name starting with EL
     > ipconfig /release *Con*        ... release all matching connections,
                                          eg. "Local Area Connection 1" or
                                              "Local Area Connection 2"
     > ipconfig /allcompartments      ... Show information about all 
                                          compartments
     > ipconfig /allcompartments /all ... Show detailed information about all
                                          compartments
