VARIANT serviceName="quickmacros2"
Services.clsServices ss._create
Services.clsService s=ss.Item(serviceName)
s.StopService ;;will end qmserv.exe process
10
s.StartService
