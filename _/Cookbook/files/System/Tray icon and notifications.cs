/// Function <see cref="script.setup"/> adds standard tray icon.

script.setup(trayIcon: true);
2.s();

/// Class <see cref="trayIcon"/> adds custom tray icon.

var ti = new trayIcon(1) { Icon = icon.trayIcon(), Tooltip = "example" };
ti.Visible = true;
ti.Click += o => { print.it("click"); };
ti.RightClick += o => { print.it("right click"); };
timer.after(2000, _ => { ti.ShowNotification("notification", "text", TINFlags.InfoIcon); });
dialog.show("tray icon"); //trayIcon works only in threads that process Windows messages; this function does it.
//wait.doEvents(30000); //another way to process messages

/// The above code could use an icon file or resource, but for simplicity it uses the script's icon, which can be changed in the Icons dialog.
