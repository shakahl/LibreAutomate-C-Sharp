typelib NETCommOCX {4580EBBB-FE3D-45CF-8543-600A62B38A73} 1.4

NETCommOCX.NETComm x._create

VARIANT d = "[0x52][0x80][0x00][0xd2]"

BSTR portsettings
x.CommPort = 3
x.Settings = portsettings
x.PortOpen = TRUE
x.Output = d

x.PortOpen = 0
