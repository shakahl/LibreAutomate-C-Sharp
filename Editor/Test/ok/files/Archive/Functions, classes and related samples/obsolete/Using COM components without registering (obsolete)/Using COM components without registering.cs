 Note: in QM 2.3.4 and later, you can instead use QM _create function with dll path. ActiveX controls also are supported.
 Note: in QM 2.3.5.1 and later you can instead use __ComActivator class.

 For those that use COM components.

 Normally, to use a COM component on a computer, it must be registered.
 You do it in QM Type Libraries dialog, or call RegisterComComponent, or use regsvr32, or run setup program, etc.
 Usually, registering a component just adds some data in the registry. Dll and type library paths, etc.
 But sometimes there is no way to register. For example, if your program does not have admin privileges.
 It is possible to use COM components without registration.

 The first way - side-by-side assemblies. Put component info in a manifest file.
    The __UseComUnregistered class uses this way.
    Advantages: It is documented and recommended way. Almost always works.
    Disadvantages: Does not work on Windows 2000 and XP SP0. The component must be in qm folder or a subfolder.

 The second way - use low level COM object creation functions.
    The CreateComObjectUnregistered uses this way.
    Advantages: Works on all Windows versions. Don't need additional files.
    Disadvantages: Works not with all components. Possible various anomalies. Cannot be used with ActiveX controls.
