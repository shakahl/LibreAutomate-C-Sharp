out
 ConvertCtoQM "$desktop$\mscoree.h" "$desktop$\mscoree.txt" "" "" 128 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"

str pd="COM_NO_WINDOWS_H 1[]__RPC_H__ 1[]__RPCNDR_H__ 1"
ConvertCtoQM "$pf$\Microsoft SDKs\Windows\v7.0\Include\mscoree.h" "$desktop$\mscoree.txt" "" pd 128|1 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"
