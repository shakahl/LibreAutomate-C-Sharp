using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Windows.Forms;
using System.Drawing;

//using System.Reflection;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

#pragma warning disable 162, 168, 219, 649 //unreachable code, unused var/field

//TODO: test: if we add all 'using' in Catkeys namespace, maybe don't need to add in each file.

public partial class Test
{
	static ImageList _TestCreateImageList()
	{
		var il = new ImageList();
		IntPtr hi = Icons.GetIconHandle(@"q:\app\browse.ico", 16);
		//il.Images.Add("k1", Icon.FromHandle(hi));
		il.Images.Add("k0", Icon.FromHandle(hi).ToBitmap());
		Api.DestroyIcon(hi);
		//il.Images.Add(SystemIcons.Exclamation); //distorted
		//il.Images.Add(Catkeys.Tasks.Properties.Resources.qm_running); //distorted, as well as with ToBitmap(), because the resource manager adds big icon
		il.Images.Add(new Icon(SystemIcons.Exclamation, 16, 16)); //distorted, the same
		return il;
	}

	static void TestCatMenuBig(Control c = null)
	{
		//Wait(1);
		var il = _TestCreateImageList();

		//CatMenu.DefaultActivateMenuWindow = true;
		//CatMenu.DefaultMouseClosingDistance = 30;

		Perf.First();
		var m = c != null ? new CatMenu(new Container()) : new CatMenu();

		Perf.Next();
		m.CMS.ImageList = il;
#if false
		m.CMS.BackColor = Color.PaleGoldenrod;
		m.CMS.ForeColor = Color.BlueViolet;
		//m.CMS.AllowTransparency=true; m.CMS.Opacity = 100; //does not work
		m.CMS.BackgroundImage = Image.FromFile(@"C:\Program Files (x86)\VideoLAN\VLC\lua\http\images\Back-48.png");
		m.CMS.BackgroundImageLayout = ImageLayout.Stretch;
		m.CMS.Cursor = Cursors.Hand;
		//m.CMS.DropShadowEnabled = false; //not useful
		m.CMS.Font = new Font("Comic Sans MS", 12);
		m.CMS.ImageScalingSize = new Size(24, 24);
		//m.CMS.MaximumSize = new Size(200, 400);
		m.CMS.ShowCheckMargin = true;
		m.CMS.ShowImageMargin=false;
		//m.CMS.ShowItemToolTips=false; //not useful

		//Out(m.CMS.Renderer); //ToolStripProfessionalRenderer
		//m.CMS.Renderer = new ToolStripSystemRenderer(); //looks more like XP menus
		//m.CMS.Renderer = new TestToolStripRenderer();
		//m.CMS.RenderMode=ToolStripRenderMode.System; //submenus auto-inherit
		//m.CMS.RightToLeft = RightToLeft.Yes; //submenus auto-inherit
#endif

		var cm = new ContextMenu();
		cm.MenuItems.Add("test").Click += (o, d) => { Out("cm"); };
		m.CMS.ContextMenu = cm;
		Perf.Next();

		m["One"] = o => Out("-one-");
		m["Two"] = o => { Out(o); };
		m.LastItem.ToolTipText = "tooltip";
		m.Submenu("Sub");
		{
			m["Three"] = o => Out("-three-");
			m["Four"] = o => Out(o);
			m.LastItem.ToolTipText = "tooltip";
			m.Submenu("Level 3", onClick: o => Out(o));
			{
				m["Five"] = o => Out(o);
#if false
				using(m.Submenu("Level 4")) {
					m["Test"] = o => Out(o);
					using(m.Submenu("Level 5")) {
						m["Test"] = o => Out(o);
						using(m.Submenu("Level 6")) {
							m["Test"] = o => Out(o);
							using(m.Submenu("Level 7")) {
								m["Test"] = o => Out(o);
								using(m.Submenu("Level 8")) {
									m["Test"] = o => Out(o);
									using(m.Submenu("Level 9")) {
										m["Test"] = o => Out(o);
									}
									m["Test"] = o => Out(o);
								}
								m["Test"] = o => Out(o);
							}
							m["Test"] = o => Out(o);
						}
						m["Test"] = o => Out(o);
					}
					m["Test"] = o => Out(o);
				}
				m["Test"] = o => Out(o);
#endif
				m.EndSubmenu();
			}
			m.Submenu("Sub2", onClick: o => Out(o));
			{
				m["Five"] = o => Out(o);
				m.EndSubmenu();
			}
			m["Six"] = o => Out(o);
			m.LastItem.ForeColor = Color.BlueViolet;
			m.EndSubmenu();
		}

		m["One"] = o => Out("-one-");
		m["Two"] = o => { Out(o); };
#if true
		using(m.Submenu("Sub with using")) {
			m["Three"] = o => Out("-three-");
			m.LastItem.Font = new Font(m.LastItem.Font, FontStyle.Bold);
			m["Four"] = o => Out(o);
			m.LastItem.Font = new Font("Tahoma", 25);
			using(m.Submenu("Sub2", onClick: o => Out(o))) {
				m["Five"] = o => Out(o);
			}
			m.Submenu("Sub2", onClick: o => Out(o));
			{
				m["Five"] = o => Out(o);
				m.EndSubmenu();
			}
			using(var smb = m.Submenu("Sub with new tooltip")) {
				smb.MenuItem.ToolTipText = "new tooltip";
				m["Five"] = o => Out(o);
			}
			m["Six"] = o => Out(o);
		}
		//m.Add(new ToolStripTextBox("txt"));
		m["Disabled"] = null;
		m.LastItem.Enabled = false;
		m["Tooltip"] = null;
		m.LastItem.ToolTipText = "ttttttttt";
		m.Add("Check", o => Out(o));
		m.LastMenuItem.Checked = true;
		var mi = m.Add("Add()", o => Out(o)); mi.BackColor = Color.AliceBlue; mi.ForeColor = Color.Orchid;
		m["Icon", @"q:\app\Cut.ico"] = o => Out(o);
		m["Icon", @"q:\app\Copy.ico"] = o => Out(o);
		m["Icon", @"q:\app\Paste.ico"] = o => Out(o);
		m["Icon", @"q:\app\Run.ico"] = o => Out(o);
		m["Icon", @"q:\app\Tip.ico"] = o => Out(o);
		//m["Icon resource", 1] = o => Out(o);
		m["Imagelist icon name", "k0"] = o => Out(o);
		m["Imagelist icon index", 1] = o => Out(o);
		using(m.Submenu("Sub3")) {
			m.LastItem.ToolTipText = "tooltip";
			m.LastItem.ForeColor = Color.Red;
			//m.LastItem.DropDown.ImageList = il;
			//m.LastItem.Margin=new Padding(8);
			m["Simple"] = o => Out(o);
			m["Icon in submenu", @"q:\app\Paste.ico"] = o => Out(o);
			m["Imagelist icon name in submenu", "k0"] = o => Out(o);
			m["Imagelist icon index in submenu", 1] = o => Out(o);
			using(m.Submenu("Sub4", "k0")) {
				m.LastItem.BackColor = Color.Bisque;
				m["Simple"] = o => Out(o);
			}
			m.Submenu("Sub5", 1);
			{
				m["Simple"] = o => Out(o);
				m.Add(new ToolStripTextBox("txt"));
				m.EndSubmenu();
			}
			m.Add(new ToolStripTextBox("txt"));
		}
		//#if false
		m.Separator();
		m.Add(new ToolStripLabel("Label"));
		m.Add(new ToolStripTextBox("txt"));
		var cb = new ToolStripComboBox("cb"); cb.Items.Add("one");
		m.Add(cb);
		//Application.EnableVisualStyles();
		cb = new ToolStripComboBox("cb"); cb.DropDownStyle = ComboBoxStyle.DropDownList; cb.Items.Add("one");
		m.Add(cb);
		m.Add(new ToolStripProgressBar("pb"));
		m.Add(new ToolStripButton("Button"));
		m.Add(new ToolStripDropDownButton("DD button"));
		m.Add(new ToolStripSplitButton("Split button"));
		m.Add(new ToolStripStatusLabel("Status label"));
		m.Add(new ToolStripMenuItem("Menu item"));
		m.LastMenuItem.ShortcutKeys = Keys.Control | Keys.K;
		m.Add(new ToolStripSeparator());

		//this code works, but the control width is several pixels
		//var ed =new TextBox();
		//ed.Width = 100;
		//var host =new ToolStripControlHost(ed, "host");
		//host.Width = 100;
		//m.Add(host);

		//test overflow
		//for(int i=0; i<30; i++) m[$"Overflow {i}"] = o => Out(o);
		m["Last"] = o => { Out(o); };

		m.LastMenuItem.MouseUp += (o, d) => { Out("MouseUp event"); };

		var tb = new ToolStripTextBox("txt");
		tb.Click += (o, oo) =>
		{
			//Out("click");
			var wo = (Wnd)tb.Owner.Handle;
			//var wo = (Wnd)tb.Control.Handle;
			//Out(wo);
			Show.TaskDialog("td", owner: wo);
			//MessageBox.Show(wo, "txt");
		};
		m.Add(tb);

		m.Submenu("Lazy", m1 =>
		{
			Out("adding items to " + m.CurrentAddMenu.OwnerItem.Text);
			m1["Lazy 1"] = o => Out(o);
			m["Lazy 2"] = o => Out(o);
			using(m.Submenu("Lazy sub")) {
				m["Lazy 3"] = o => Out(o);
			}
			m["Lazy 4"] = o => Out(o);
		});

		m.Submenu("Lazy lazy", m1 =>
		{
			Out("adding items to " + m.CurrentAddMenu.OwnerItem.Text);
			m.CurrentAddMenu.BackColor = Color.Beige;
			m1["Lazy 1"] = o => Out(o);
			m["Lazy 2"] = o => Out(o);
			m.Submenu("Lazy sub", m2 =>
			{
				Out("adding lazy lazy items");
				using(m.Submenu("Lazy sub")) {
					m["Lazy 3"] = o => Out(o);
				}
				m["Lazy 3"] = o => Out(o);
			});
			m["Lazy 4"] = o => Out(o);
		});

		Perf.Next();

		//GC.Collect(); GC.Collect();

		//m.CMS.Items.Add("TTTTT", null, (o, oo) => Out(10));
		//m.CMS.ResumeLayout();
		//m.CMS.Show();
		//m.CMS.Show(c, 0, 0);
		//return;
#endif

		//c.ContextMenuStrip = m.CMS;
		//return;

		//m.ActivateMenuWindow = true;
		//m.MultiShow = true;
		if(c != null) {
			//m.ModalAlways = true;
			m.Show(c, 100, 100);
			//Show.TaskDialog("");
			//m.Show(c, 200, 200);
		} else {
			//Wait(1);
			//Out(1);
			//Perf.Next();
			//m.Show(500, 300);
			//m.Show();
			m.Show(Mouse.X + 10, Mouse.Y + 10);
			//Out(2);
			//WaitMS(200); Perf.First(); m.Show(); Out(3);
		}
	}

	class TestToolStripRenderer :ToolStripProfessionalRenderer
	{
		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			base.OnRenderToolStripBorder(e);
			ControlPaint.DrawBorder(e.Graphics, e.AffectedBounds, Color.Orange, ButtonBorderStyle.Solid);
		}
	}

	static void TestCatMenuIcons()
	{
		//Out(Api.GetCurrentThreadId());

		//Environment.CurrentDirectory = @"q:\app";

		var a = new List<string>();
		int n = 0;
		bool lnk = false;
		string folder;

		//if(lnk) {
		//	folder = Folders.CommonPrograms;
		//	foreach(var f in Directory.EnumerateFiles(folder, "*.lnk", System.IO.SearchOption.AllDirectories)) {
		//		//Out(f);
		//		a.Add(f);
		//		if(++n == 30) break;
		//	}
		//} else {
		//	folder = @"q:\app";
		//	//folder =@"q:\app\catkeys\tasks";
		//	var oneExt = new HashSet<string>();
		//	foreach(var f in Directory.EnumerateFiles(folder)) {
		//		//Out(f);
		//		var ext = Path.GetExtension(f).ToLower(); if(oneExt.Contains(ext)) continue; else oneExt.Add(ext);
		//		if(0 != f.EndsWith_(true, ".aps", ".tss", ".bin", ".wal")) continue;
		//		a.Add(f);
		//		if(++n == 30) break;
		//	}
		//}

		Perf.First();

		var m = new CatMenu();
		m.MouseClosingDistance = 50;
		//m.ActivateMenuWindow = true;
		m.IconDirectory = @"q:\app";
		//m.CMS.ImageScalingSize = new Size(32, 32);
		//m.CMS.ImageScalingSize = new Size(48,48);
		//m.CMS.ImageScalingSize = new Size(64,64);
		//m.CMS.ImageScalingSize = new Size(256,256);
		//m.CMS.ImageScalingSize = new Size(24,24);

		foreach(var f in a) {
			//Out(f);
			m[Path.GetFileName(f), f] = null;
			//m[Path.GetFileName(f), Path.GetFileName(f)] = null;
			//m[Path.GetFileName(f), @"C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\vcpackages\VCProject.dll,2"] = null;

			////Task.Run(() =>
			////{
			//	var hi = Files.GetIconHandle(f);
			//	if(hi != Zero) Api.DestroyIcon(hi);
			//	else Out("no icon: " + f);
			////});
		}

		//using(m.Submenu("sub")) {
#if true
		m["One", "Cut.ico"] = o => Out(o);
		m["ILSpy", @"Q:\Programs\ILSpy\ILSpy.exe"] = o => Out(o);
		m["Notepad", Folders.System + "notepad.exe"] = o => Out(o);
		m["Two", "Copy.ico"] = o => Out(o);
		m["Three", "Paste.ico"] = o => Out(o);
		m["Four", "Run.ico"] = o => Out(o);
		m["Five", "Tip.ico"] = o => Out(o);
		//m["Six", "notepad.exe"] = o => Out(o);
		m["Calc", @"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"] = o => Out(o);
		m["PicPick", Folders.ProgramFilesX86 + @"PicPick\picpick.exe"] = o => Out(o);
		m["Dbgview", @"Q:\Programs\DebugView\Dbgview.exe"] = o => Out(o);
		m["Procexp", @"Q:\Programs\ProcessExplorer\procexp.exe"] = o => Out(o);
		m["Inno", Folders.ProgramFilesX86 + @"Inno Setup 5\Compil32.exe"] = o => Out(o);
		m["hhw", Folders.ProgramFilesX86 + @"HTML Help Workshop\hhw.exe"] = o => Out(o);
		m["FileZilla", Folders.ProgramFilesX86 + @"FileZilla FTP Client\filezilla.exe"] = o => Out(o);
		m["IE", Folders.ProgramFilesX86 + @"Internet Explorer\IEXPLORE.EXE"] = o => Out(o);
		m["Procmon", @"Q:\Programs\ProcessMonitor\Procmon.exe"] = o => Out(o);
		m["ResourceHacker", Folders.ProgramFilesX86 + @"Resource Hacker\ResourceHacker.exe"] = o => Out(o);
		m["autoruns", @"Q:\programs\Autoruns\autoruns.exe"] = o => Out(o);
		m["SyncBack", Folders.ProgramFilesX86 + @"SyncBackFree\SyncBackFree.exe"] = o => Out(o);
		m["PEview", @"Q:\Programs\PeView\PEview.exe"] = o => Out(o);
		m["shell32.dll,25", Folders.System + @"shell32.dll,25"] = o => Out(o);
		m["app", @"q:\app"] = null;
		m["Favorites", Folders.Favorites] = o => Out(o);
		//m["", @""] = o => Out(o);
		m["http://www...", "http://www.quickmacros.com/"] = o => Out(o);
		m[".txt", ".txt"] = o => Out(o);
		m[".bmp", ".bmp"] = o => Out(o);
		m["mailto:", "mailto:"] = o => Out(o);
		m["CLSID", "::{21EC2020-3AEA-1069-A2DD-08002B30309D}"] = o => Out(o);
		m["ProgId", "Word.Document.8"] = o => Out(o);
		m["lnk", @"C:\Users\G\Desktop\QM in PF.lnk"] = o => Out(o);
		m[".. exe", @"q:\app\catkeys\..\qm.exe"] = null;
		m[".. cpp", @"q:\app\catkeys\..\app.cpp"] = null;
		m["txtfile:", @"txtfile:"] = null;
		m[".cat", @".cat"] = null;
		m[".cat", @".cat"] = null;
		m["ani", @"C:\WINDOWS\Cursors\aero_busy.ani"] = null;
#endif

		m["no icon"] = null;
			m["no icon"] = null;
			m.Separator();
			//using(m.Submenu("sub")) {
			m["no icon"] = null;
			m["no icon"] = null;
			m[".cat", @".cat"] = null;
		//}

		//Time.SetTimer(1000, true, t => { m.CMS.Close(); });
		//Time.SetTimer(1000, true, t => { m.CMS.Visible=false; });
		//Time.SetTimer(1000, true, t => { m.Dispose(); });
		//Time.SetTimer(1000, true, t => { m.CMS.Dispose(); });

		//m.MultiShow = true;
		m.Show();
		//WaitMS(500);
		//m.Separator();
		//m["plus", @"q:\app\cut.ico"] = null;
		//m["plus", @"q:\app\copy.ico"] = null;
		//m["plus", @"q:\app\paste.ico"] = null;
		//m.Show(Mouse.X + 10, Mouse.Y);

		//Out(1);
		//Show.TaskDialog("");
		//Out(2);

		//using(m) { }
	}

	static void TestCatMenuSimplest(Control c = null)
	{
		Perf.First();

		var m = new CatMenu();
		m["One"] = o => Out(o);
		m["Two"] = o => Out(o);
		using(m.Submenu("Submenu")) {
			m["Three"] = o => Out(o);
			m["Four"] = o => Out(o);
			using(m.Submenu("Submenu")) {
				m["Five"] = o => Out(o);
				m["Six"] = o => Out(o);
			}
			m["Seven"] = o => Out(o);
		}
		m["Eight"] = o => Out(o);
		m.Show();

		//var m = new CatMenu();
		//m["One"] = o => Out(o);
		//m["Two"] = o => Out(o);
		//m.Submenu("Submenu 1", m1 =>
		//{
		//	Out("adding items of " + m.CurrentAddMenu.OwnerItem);
		//	m["Three"] = o => Out(o);
		//	m["Four"] = o => Out(o);
		//	m.Submenu("Submenu 2", m2 =>
		//	{
		//		Out("adding items of " + m.CurrentAddMenu.OwnerItem);
		//		m["Five"] = o => Out(o);
		//		m["Six"] = o => Out(o);
		//	});
		//	m["Seven"] = o => Out(o);
		//});
		//m["Eight"] = o => Out(o);
		//m.Show();

		//var m = new CatMenu();
		//m.Add("One", o => Out(o), @"icon file path");
		//m.Add("Two", o => { Out(o.MenuItem.Checked); });
		//m.LastMenuItem.Checked = true;

		//m.Separator();
		//m.Add(new ToolStripLabel("Label"), null, o => Out(o));
		//m.Add(new ToolStripTextBox("txt"), null, o => Out(o));
		//var cb = new ToolStripComboBox("cb"); cb.Items.Add("one");
		//m.Add(cb, null, o => Out(o));
		//m.Add(new ToolStripProgressBar("pb"), null, o => Out(o));
		//m.Add(new ToolStripButton("Button"), null, o => Out(o));
		//m.Add(new ToolStripDropDownButton("DD button"), null, o => Out(o));
		//m.Add(new ToolStripSplitButton("Split button"), null, o => Out(o));
		//m.Add(new ToolStripStatusLabel("Status label"), null, o => Out(o));
		//m.Add(new ToolStripMenuItem("Menu item"), null, o => Out(o));

		//m.Show();

#if false
		var b = new CatBar();
		b["One"] = o => Out(o);

		using(b.DropDownButton("Drop")) {
			b["Two"] = o => Out(o);
		}

		using(b.SplitButton("Split")) {
			b["Two"] = o => Out(o);
		}

		using(var m as b.SplitButton("Drop")) {
			m["Two"] = o => Out(o);
		}

		var m = b.DropMenu("Drop"));
		m["Two"] = o => Out(o);

		b["ToolStripDropDownButton"] = o =>
		{
			var m = new CatMenu();
			m["One"] = o => Out(o);
			m["Two"] = o => Out(o);
			m.Show(o.Item);
		};

		b["ToolStripDropDownButton"] = null;  //test ShowDropDown
		{
			var m = new CatMenu();
			m["One"] = o => Out(o);
			m["Two"] = o => Out(o);
			m.Show(o.Item);
			(b.LastItem as ToolStripDropDownButton).DropDown = m.CMS;
        };

		b["ToolStripSplitButton"] = o =>
		{
			var m = new CatMenu();
			m["One"] = o => Out(o);
			m["Two"] = o => Out(o);
			m.Show(b.LastItem);
			//or
			(o.Item as ToolStripSplitButton).DropDown = m.CMS;
		}
		//(b.LastItem as ToolStripSplitButton).ButtonClick+=(o,oo)=>Out(o);
		//b.LastButton.Click=o=>Out(o);

		b["ToolStripSplitButton"] = o => Out(o);
		{
			var m = new CatMenu();
			m["One"] = o => Out(o);
			m["Two"] = o => Out(o);
			(b.LastItem as ToolStripSplitButton).DropDown = m.CMS;
		}

		//b.SplitButton("text", onClick, onDropDown, icon);
		//b.SplitButton("text", onClick, dropDownMenu, icon);
#endif
	}

	class Form1 :Form
	{
		public Form1()
		{
			//this.MouseClick += (sender, e) =>
			//{
			//	//Out(e.Button); //no right-click event if a context menu assigned
			//	//if(e.Button == MouseButtons.Right) Wnd.FindRaw("QM_Editor").ActivateRaw();
			//	//TestCatMenu(sender as Form);
			//	Time.SetTimer(100, true, o => TestCatMenu(sender as Form));
			//};

			TestCatMenu(this);

			TestCatMenuWithToolStrip();

			//test as Control's context menu
#if false
			var m = new CatMenu();

			//return;
			m["One f"] = o => Out(o);
			using(m.Submenu("Sub f")) {
				m["Two f"] = o => Out(o);
			}
			m.Add(new ToolStripTextBox());
			this.ContextMenuStrip = m.CMS;
#else
#endif
		}

		void TestCatMenuWithToolStrip()
		{
			var ts = new ToolStrip();
			var ddb = new ToolStripDropDownButton("DD");
			ts.Items.Add(ddb);
			this.Controls.Add(ts);
			ddb.DropDownOpening += (unu, sed) =>
			{
				if(ddb.HasDropDownItems) return;
				Out("adding items");
				var m = new CatMenu();
				ddb.DropDown = m.CMS;
				m["One"] = o => Out(o);
				using(m.Submenu("Sub")) {
					m["Two"] = o => Out(o);
				}
				m.Add(new ToolStripTextBox());
			};
		}

		protected override void WndProc(ref Message m)
		{
			if(_outMsg) Util.Debug_.OutMsg(ref m);

			base.WndProc(ref m);
		}

		bool _outMsg;

	}

	static void TestCatMenuWithForm()
	{
		Perf.First();
		new Form1().ShowDialog();

		//with this loop does not show f
		//Api.MSG u;
		//while(Api.GetMessage(out u, Wnd0, 0, 0) > 0) {
		//	Api.TranslateMessage(ref u);
		//	Api.DispatchMessage(ref u);
		//}
	}

	//class TestDtor
	//{
	//	public ContextMenuStrip _cm;

	//	public TestDtor()
	//	{
	//		//_cm = new ContextMenuStrip();
	//		_cm = new ContextMenuStrip_(this);
	//	}
	//	~TestDtor() { Out("~TestDtor"); }

	//	void Met() { OutFunc(); }

	//	class ContextMenuStrip_ :ContextMenuStrip
	//	{
	//		WeakReference<TestDtor> _cat;
	//		//TestDtor _cat;

	//		internal ContextMenuStrip_(TestDtor cat)
	//		{
	//			_cat = new WeakReference<TestDtor>(cat);
	//			TestDtor c;
	//			if(_cat.TryGetTarget(out c)) c.Met();
	//		}
	//	}
	//}

	class TestDtor
	{
		IContainer _components;
		ContextMenuStrip_ _cm;

		public ContextMenuStrip CMS { get { return _cm; } }

		public TestDtor()
		{
			//_cm = new ContextMenuStrip();
			//_cm = new ContextMenuStrip_(this);

			_components = new Container();
			_cm = new ContextMenuStrip_(this, _components);
		}
		~TestDtor() { Out("~TestDtor"); }

		void Met() { OutFunc(); }

		class ContextMenuStrip_ :ContextMenuStrip
		{
			TestDtor _cat;
			//WeakReference<TestDtor> _catWeakRef;

			internal ContextMenuStrip_(TestDtor cat)
			{
				_cat = cat;
				//_catWeakRef = new WeakReference<TestDtor>(cat);
			}

			internal ContextMenuStrip_(TestDtor cat, IContainer container) : base(container)
			{
				_cat = cat;
				//_catWeakRef = new WeakReference<TestDtor>(cat);
			}

			protected override void OnOpened(EventArgs e)
			{
				base.OnOpened(e);

				//_cat.Met();
				//TestDtor c;
				//if(_cat.TryGetTarget(out c)) c.Met();
			}

			protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
			{
				base.OnClosed(e);

				//Api.DestroyWindow((Wnd)Handle);

				//_cat = null;

				OutFunc();

				((Wnd)Handle).Post(Api.WM_CLOSE);

			}

			protected override void WndProc(ref Message m)
			{
				Util.Debug_.OutMsg(ref m);

				base.WndProc(ref m);

				if(m.Msg == (int)Api.WM_SHOWWINDOW && (int)m.WParam == 0) {

				}
			}
		}
	}

	class TestDtor2 :ContextMenuStrip
	{

		public ContextMenuStrip CMS { get { return this; } }

		~TestDtor2() { Out("~TestDtor2"); }

		void Met() { OutFunc(); }
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCatMenuDtors()
	{
		var m = new CatMenu();
		m["One"] = o => Out(o);
		using(m.Submenu("sub")) {
			m["Two"] = o => Out(o);
		}
		m.MultiShow = true;
		//m.MouseClosingDistance = 2000;
		//var k = new ContextMenu(); k.MenuItems.Add("context"); m.CMS.ContextMenu = k;
		//Time.SetTimer(2000, true, t => m.Close());
		//Time.SetTimer(2000, true, t => m.Dispose());
		//Time.SetTimer(2000, true, t => m.CMS.Close());
		//Time.SetTimer(2000, true, t => m.CMS.Dispose());
		m.Show();
		//Out(1);
		//GC.Collect();
		//WaitMS(500);
		//Out(2);
		m.Show();

		//var f = new Form();
		//f.Load += (unu, sed) =>
		//  {
		//	  f.ContextMenuStrip = m.CMS;
		//  };
		//f.Click += (unu, sed) =>
		//  {
		//	  m.Show(f, 100, 100);
		//  };
		//f.ShowDialog();


		//		var t = new TestDtor();
		//		t.CMS.Items.Add("test");
		//#if true
		//		//t.CMS.Closed += (unu, sed) => Application.ExitThread();
		//		t.CMS.Closed += (unu, sed) => _loop.Stop();
		//		//ToolStripDropDownClosedEventHandler del= (unu, sed) => _loop.Stop(); t.CMS.Closed += del;
		//        for(int i = 0; i < 1; i++) {
		//			t.CMS.Show();
		//			//Application.Run();
		//			_loop.Loop();
		//			Out("after loop");

		//			//Api.DestroyWindow((Wnd)t.CMS.Handle);
		//			//Out(t.CMS.IsHandleCreated);
		//			//if(t.CMS.IsHandleCreated) Out(((Wnd)t.CMS.Handle).IsValid);

		//			//t.CMS.Closed -= del;

		//			//t.CMS.Dispose();
		//			//Wnd w = (Wnd)t.CMS.Handle;
		//			//w.Send(Api.WM_CLOSE);
		//		}
		//#endif
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCatMenuDtors2()
	{
		var t = new ContextMenuStrip();
		t.Items.Add("test");
#if true
		//t.Closed += (unu, sed) => Application.ExitThread();
		t.Closed += (unu, sed) => _loop.Stop();
		t.Disposed += (unu, sed) => Out("disposed");
		t.Show();
		//Application.Run();
		_loop.Loop();
#endif
	}

	//Util.MessageLoop _loop = new Util.MessageLoop();

	static void TestCatMenuCommonAddCallback()
	{
		var m = new CatMenu();

		m.ItemAdded += x => { Out(x.Text); if(x.Text.Length > 3) x.BackColor = Color.Azure; };

		m["One"] = o => Out(o);
		m["Two"] = o => Out(o);
		using(m.Submenu("Submenu")) {
			m["Three"] = o => Out(o);
			m["Four"] = o => Out(o);
			using(m.Submenu("Submenu")) {
				m["Five"] = o => Out(o);
				m["Six"] = o => Out(o);
			}
			m["Seven"] = o => Out(o);
		}
		m["Eight"] = o => Out(o);
		m.Show();
	}

	static void TestCatMenu(Control c = null)
	{
		//if(c == null) WaitMS(200);
		//c = null;
		for(int i = 0; i < 1; i++) {
			//TestCatMenuBig(c);
			//TestCatMenuSimplest(c);
			//TestCatMenuDtors();
			TestCatMenuIcons();
			//TestCatMenuCommonAddCallback();

			//TestWpfContextMenu();

			if(c != null) break;
			//Application.DoEvents();
			WaitMS(100);
			//Application.DoEvents();
			//WaitMS(1000);
		}

		//for(int i = 0; i < 1; i++) {
		//	GC.Collect();
		//	Show.TaskDialog("f");
		//}
	}



	#region test old toolbar

	static void TestOldToolbar()
	{
		Perf.First();
		var f = new CatBar1();
		//f.Icon = null;
		f.Height = 50; f.Width = 1200;


		var t = new ToolBar();
		//t.SuspendLayout();
		//t.Size = new Size(250, 25);
		t.Divider = false;
		t.ButtonClick += T_ButtonClick;
		Perf.Next();

		var b = new ToolBarButton("Text");
		t.Buttons.Add(b);
		Perf.Next();

		for(int i = 0; i < 30; i++) {
			//t.Buttons.Add("Text");
			b = new ToolBarButton("Text");
			t.Buttons.Add(b);
		}
		Perf.Next();

		f.Controls.Add(t);
		//t.ResumeLayout();
		Perf.Next();
		//f.Show();
		//f.Visible = true;
		Wnd w = (Wnd)f;
		w.Visible = true;
		//w.ActivateRaw();
		Perf.Next();

		//Application.Run();
		_mlTb.Loop();
		f.Close();
		f.Dispose();
	}

	private static void T_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
	{
		OutFunc();
		_mlTb.Stop();
	}

	static Util.MessageLoop _mlTb = new Util.MessageLoop();

	#endregion

	#region test toolbar strip

	public class CatBar1 :Form
	{
		protected override CreateParams CreateParams
		{
			get
			{
				var p = base.CreateParams;
				p.Style = unchecked((int)Api.WS_POPUP);
				p.ExStyle = (int)(Api.WS_EX_TOOLWINDOW | Api.WS_EX_NOACTIVATE | Api.WS_EX_TOPMOST);
				//p.Height = 50; p.Width = 1200;
				p.X = 400; p.Y = 200;
				//p.ClassName = _tbWndClass.Name;
				return p;
			}
		}
	}

	static LPARAM _WndprocCatBar(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		//Out(msg);
		//switch(msg) {
		//case Api.WM_DESTROY:
		//	break;
		//}

		return Api.DefWindowProc(w, msg, wParam, lParam);
	}
	static Wnd.Misc.WndClass _tbWndClass;

	static void TestToolbarStrip()
	{
		if(_tbWndClass == null) {
			_tbWndClass = Wnd.Misc.WndClass.Register("CatBar1", _WndprocCatBar, 0, Api.CS_GLOBALCLASS);
		}

		Perf.First();
		var f = new CatBar1();
		//f.SuspendLayout();
		//f.Icon = null;
		f.Height = 50; f.Width = 1200; //faster here than in CreateParams

		Perf.Next();
		var t = new ToolStrip();
		t.SuspendLayout();
		Perf.Next();

		var b = new ToolStripButton("Text");
		b.Click += B_Click1;
		t.Items.Add(b);
		Perf.Next();

		for(int i = 0; i < 30; i++) {
			//t.Items.Add("Text");
			b = new ToolStripButton("Text");
			t.Items.Add(b);
		}
		Perf.Next();

		f.Controls.Add(t);
		t.ResumeLayout();
		Perf.Next();
		//Wnd w = (Wnd)f; w.Visible = true; //slightly faster, but then need 2 clicks to make a button to respond
		f.Show(); //does not activate if WS_EX_NOACTIVATE
		Perf.Next();

		//Application.Run();
		_mlTb.Loop();
		f.Close();
		f.Dispose();
	}

	private static void B_Click1(object sender, EventArgs e)
	{
		OutFunc();
		_mlTb.Stop();
	}

	#endregion

	#region test toolbar strip in native window

	static LPARAM _WndprocCatBar2(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		//Out(msg);
		switch(msg) {
		case Api.WM_DESTROY:
			_mlTb.Stop();
			break;
		}

		LPARAM R = Api.DefWindowProc(w, msg, wParam, lParam);

		switch(msg) {
		case Api.WM_PAINT:
			//Out("painted");
			//if(_tbStrip2 != null) {
			//	ToolStrip2 t = _tbStrip2; _tbStrip2 = null;
			//	if(!t.Focused) t.Focus(); //solves problem when in native window: the first button-click does not work. This takes several milliseconds therefore is after painting.
			//}
			break;
		}

		return R;
	}

	static Wnd.Misc.WndClass _tbWndClass2;

	class ToolStrip2 :ToolStrip
	{
		IntPtr _hwndParent;

		public ToolStrip2(IntPtr hwndParent) { _hwndParent = hwndParent; }

		protected override CreateParams CreateParams
		{
			get
			{
				var p = base.CreateParams;
				p.Parent = _hwndParent;
				return p;
			}
		}
	}

	static void TestToolbarStripInNativeWindow()
	{
		Perf.First();
		if(_tbWndClass2 == null) {
			_tbWndClass2 = Wnd.Misc.WndClass.Register("CatBar2", _WndprocCatBar2, IntPtr.Size, Api.CS_GLOBALCLASS);
			Perf.Next();
		}

		//bool topMost = true;
		Wnd w = Api.CreateWindowEx(Api.WS_EX_TOOLWINDOW | Api.WS_EX_NOACTIVATE | Api.WS_EX_TOPMOST, _tbWndClass2.Name, null,
			Api.WS_POPUP | Api.WS_CAPTION | Api.WS_SYSMENU, 400, 200, 1200, 80, Wnd0, 0, Zero, 0);
		Perf.Next();

#if true
		//var t = new ToolStrip();
		var t = new ToolStrip2(w.Handle);
		t.SuspendLayout();
		t.SetBounds(0, 0, 1100, 40);
		Perf.Next();

		var b = new ToolStripButton("Text");
		b.Click += B_Click1;
		t.Items.Add(b);
		Perf.Next();

		for(int i = 0; i < 30; i++) {
			//t.Items.Add("Text");
			b = new ToolStripButton("Text");
			t.Items.Add(b);
		}
		Perf.Next();

		//Wnd wt = (Wnd)t.Handle;
		//Out(wt);
		//Out(wt.DirectParent);
		//if(Api.SetParent(wt, w).Is0) Out(new Win32Exception().Message);
		//Out(wt.DirectParentOrOwner);

		t.ResumeLayout();
		t.CreateControl();
		//Wnd wt = (Wnd)t.Handle;

		Perf.Next();
#endif
		w.Visible = true;
		//w.ActivateRaw();
		//Perf.Next();
		//Wnd wt = (Wnd)t.Handle;
		//w.Send(Api.WM_ACTIVATE, 1); w.Send(Api.WM_ACTIVATE, 0); //solves problem when in native window: the first button-click does not work
		//w.Post(Api.WM_ACTIVATE, 1); w.Post(Api.WM_ACTIVATE, 0);
		//t.Select();
		//Perf.Next();
		//t.Focus();
		Perf.Next();
		_mlTb.Loop();
		w.Destroy();
		//if(!t.IsDisposed) t.Dispose();
	}

	#endregion

	#region test old toolbar in native window

	static LPARAM _WndprocCatBar3(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		//Out(msg);
		switch(msg) {
		case Api.WM_DESTROY:
			_mlTb.Stop();
			break;
		}

		return Api.DefWindowProc(w, msg, wParam, lParam);
	}

	static Wnd.Misc.WndClass _tbWndClass3;

	class ToolBar2 :ToolBar
	{
		IntPtr _hwndParent;

		public ToolBar2(IntPtr hwndParent) { _hwndParent = hwndParent; }

		protected override CreateParams CreateParams
		{
			get
			{
				var p = base.CreateParams;
				p.Parent = _hwndParent;
				return p;
			}
		}
	}

	static void TestOldToolbarInNativeWindow()
	{
		if(_tbWndClass3 == null) {
			_tbWndClass3 = Wnd.Misc.WndClass.Register("CatBar3", _WndprocCatBar2, 0, Api.CS_GLOBALCLASS);
		}

		Perf.First();
		//bool topMost = true;
		Wnd w = Api.CreateWindowEx(Api.WS_EX_TOOLWINDOW | Api.WS_EX_NOACTIVATE | Api.WS_EX_TOPMOST, _tbWndClass3.Name, null,
			Api.WS_POPUP | Api.WS_CAPTION | Api.WS_SYSMENU, 400, 200, 1200, 80, Wnd0, 0, Zero, 0);
		Perf.Next();


		var t = new ToolBar2(w.Handle);
		//t.SuspendLayout();
		t.Size = new Size(1100, 40);
		t.Divider = false;
		t.ButtonClick += T_ButtonClick;
		Perf.Next();

		var b = new ToolBarButton("Text");
		t.Buttons.Add(b);
		Perf.Next();

		for(int i = 0; i < 30; i++) {
			//t.Buttons.Add("Text");
			b = new ToolBarButton("Text");
			t.Buttons.Add(b);
		}
		Perf.Next();

		//t.ResumeLayout();
		t.CreateControl();
		//Wnd wt = (Wnd)t.Handle;

		Perf.Next();
		w.Visible = true;
		//w.ActivateRaw();
		Perf.Next();
		_mlTb.Loop();
		w.Destroy();
	}

	#endregion

	#region test native window speed

	static LPARAM _WndprocNW(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		//Out(msg);
		switch(msg) {
		case Api.WM_DESTROY:
			_mlTb.Stop();
			break;
		}

		LPARAM R = Api.DefWindowProc(w, msg, wParam, lParam);

		return R;
	}

	static Wnd.Misc.WndClass _WndClassNW;

	static void TestNativeWindow()
	{
		Perf.First();
		if(_WndClassNW == null) {
			_WndClassNW = Wnd.Misc.WndClass.Register("NativeWi", _WndprocNW, IntPtr.Size, Api.CS_GLOBALCLASS);
			Perf.Next();
		}

		//bool topMost = true;
		Wnd w = Api.CreateWindowEx(Api.WS_EX_TOOLWINDOW | Api.WS_EX_NOACTIVATE | Api.WS_EX_TOPMOST, _WndClassNW.Name, null,
			Api.WS_POPUP | Api.WS_CAPTION | Api.WS_SYSMENU, 400, 200, 1200, 80, Wnd0, 0, Zero, 0);
		Perf.Next();
		w.Visible = true;
		//w.ActivateRaw();
		//Perf.Next();
		//Wnd wt = (Wnd)t.Handle;
		//w.Send(Api.WM_ACTIVATE, 1); w.Send(Api.WM_ACTIVATE, 0); //solves problem when in native window: the first button-click does not work
		//w.Post(Api.WM_ACTIVATE, 1); w.Post(Api.WM_ACTIVATE, 0);
		//t.Select();
		//Perf.Next();
		//t.Focus();
		Perf.Next();
		_mlTb.Loop();
		w.Destroy();
		//if(!t.IsDisposed) t.Dispose();
	}

	#endregion


	static void TestCatBar()
	{
		//var il = _TestCreateImageList();

		Perf.First();
		var m = new CatBar();
		//m.ImageList = il;
		Perf.Next();

		m.Ex.SetBounds(100, 100, 400, 100);

		m["Close"] = o => { Out(o); _mlTb.Stop(); };
		Perf.Next();
#if true
		for(int i = 0; i < 30; i++) {
			m.Add("Text", null);
		}
#else
		m.Separator();
		m["Icon", @"q:\app\Cut.ico"] = o => Out(o);
		m["Icon", @"q:\app\Copy.ico"] = o => Out(o);
		m["Icon", @"q:\app\Paste.ico"] = o => Out(o);
		m["Icon", @"q:\app\Run.ico"] = o => Out(o);
		m["Icon", @"q:\app\Tip.ico"] = o => Out(o);
		m.LastItem.ForeColor = Color.OrangeRed;
		//m["Icon resource", 1] = o => Out(o);
		m["Imagelist icon name", "k0"] = o => Out(o);
		m["Imagelist icon index", 1, tooltip:"tooltip"] = o => Out(o);
		m.Separator();
		m.Add(new ToolStripTextBox());
#endif
		Perf.Next();

		//Out(w);
		m.Visible = true;
		Perf.Next();
		_mlTb.Loop();
		m.Close();
	}

	static void TestToolbar()
	{
		//OutFunc();
		for(int i = 0; i < 1; i++) { TestCatBar(); /*WaitMS(500);*/ }

		//for(int i=0; i<1; i++) TestOldToolbar();
		//for(int i=0; i<1; i++) TestOldToolbarInNativeWindow();
		//for(int i=0; i<1; i++) TestToolbarStrip();
		//for(int i=0; i<1; i++) TestToolbarStripInNativeWindow();
		//for(int i=0; i<4; i++) TestNativeWindow();

		//TestWpfToolbar();
		//TestWpfToolbar();
		//var t = new Thread(() => { TestWpfToolbar(); });
		//t.SetApartmentState(ApartmentState.STA); //WPF menu does not work without this
		//t.Start();
		//t.Join();
	}


	static void TestMenuTB()
	{

		//MessageBox.Show("");
		//Show.TaskDialog("");

		//WaitMS(500);
		TestToolbar();
		//TestCatMenu();
		//TestCatMenuWithForm();
		//WaitMS(500);
		//TestWpfContextMenu();
		//WaitMS(500);
		//TestWpfContextMenu();

		//var t = new Thread(() => { TestWpfContextMenu(); });
		//t.SetApartmentState(ApartmentState.STA); //WPF menu does not work without this
		//t.Start();
		//t.Join();

		//MessageBox.Show(""); //after menu this is behind other windows. Form too.
		//Show.TaskDialog(""); //active, OK

		//Form f=new Form();
		//f.Load += (unu, sed)=>{ f.Activate(); }; //OK
		//f.ShowDialog();
	}
}


//Application.AddMessageFilter
//Application.DoEvents
//Application.ExecutablePath
//Application.ExitThread
//Application.FilterMessage
//Application.MessageLoop
//Application.OleRequired
//Application.OpenForms
//Application.RegisterMessageLoop
//Application.SetSuspendState
//Application.SetUnhandledExceptionMode
//Application.StartupPath
//Application.ThreadException event
//Application.UnregisterMessageLoop
