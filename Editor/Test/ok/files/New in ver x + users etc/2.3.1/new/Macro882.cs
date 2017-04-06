str sf="$qm$\System.qml"
SetAttr sf FILE_ATTRIBUTE_READONLY 2 ;;remove read-only attribute
ren+ sf "$qm$\System - backup (2-3-0-13).qml" ;;backup System.qml
zip- "$desktop$\System.zip" "$qm$" ;;extract the zip file
shutdown -2 ;;restart qm
