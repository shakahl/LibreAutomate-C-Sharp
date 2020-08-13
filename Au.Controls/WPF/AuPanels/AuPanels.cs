using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Markup;

using Au.Types;

namespace Au.Controls.WPF
{
	/// <summary>
	/// Creates and manages window layout like in Visual Studio.
	/// Multiple docked movable/sizable/tabable/floatable/hidable/savable panels with splitters where you can add controls.
	/// </summary>
	/// <remarks>
	/// Layout is defined in default XML file, then saved in other XML file. See Layout.xml in Aedit project.
	/// Let your window's ctor call <see cref="Load"/>, set content of all panels/toolbars like <c>_panels["Files"].Content = new TreeView();</c>, and set window Content = <see cref="RootElem"/>.
	/// Let your app call <see cref="Save"/> when closing window or at any time before it.
	/// If new app version adds/removes/renames panels/toolbars, this class automatically updates the saved layout.
	/// </remarks>
	public partial class AuPanels
	{
		List<_Stack> _aStack = new();
		List<_Panel> _aPanel = new();
		_Stack _rootStack;
		string _xmlFile;

		/// <summary>
		/// Loads layout from XML file.
		/// </summary>
		/// <param name="xmlFileDefault">XML file containing default layout. It eg can be in AFolders.ThisApp.</param>
		/// <param name="xmlFileCustomized">XML file containing user-modified layout. It will be created or updated by <see cref="Save"/>.</param>
		public void Load(string xmlFileDefault, string xmlFileCustomized) {
			if (_xmlFile != null) throw new InvalidOperationException();
			_xmlFile = xmlFileCustomized;

			//At first try to load xmlFileCustomized. If it does not exist or is invalid, load xmlFileDefault.
			string xmlFile = xmlFileCustomized; bool useDefaultXML = false;
			gRetry:
			if (useDefaultXML) xmlFile = xmlFileDefault;
			else if (useDefaultXML = !AFile.ExistsAsFile(xmlFile)) goto gRetry;

			try {
				var x = AExtXml.LoadElem(xmlFile).Element("stack");
				if (!useDefaultXML) _AutoUpdateXml(x, xmlFileDefault);
				new _Stack(this, null, x); //creates all and sets _rootStack
			}
			catch (Exception e) when (!Debugger.IsAttached) {
				var sErr = $"Failed to load file '{xmlFile}'.\r\n\t{e.ToStringWithoutStack()}";
				if (useDefaultXML) {
					_xmlFile = null;
					ADialog.ShowError("Cannot load window layout from XML file.", $"{sErr}\r\n\r\nReinstall the application.");
					Environment.Exit(1);
				} else {
					AWarning.Write(sErr, -1);
				}
				_aStack.Clear(); _aPanel.Clear(); _rootStack = null;
				useDefaultXML = true; goto gRetry;
			}
		}

		/// <summary>
		/// Saves layout to XML file <i>xmlFileCustomized</i> specified when calling <see cref="Load"/>.
		/// </summary>
		public void Save() {
			try {
				AFile.CreateDirectoryFor(_xmlFile);
				var sett = new XmlWriterSettings() {
					OmitXmlDeclaration = true,
					Indent = true,
					IndentChars = "\t"
				};
				AFile.Save(_xmlFile, temp => {
					using var x = XmlWriter.Create(temp, sett);
					x.WriteStartDocument();
					x.WriteStartElement("panels");
					_rootStack.Save(x);
					x.WriteEndDocument();
				});
				//AFile.Run("notepad.exe", _xmlFile); ATimer.After(1000, _ => DeleteSavedFile());
			}
			catch (Exception ex) { AOutput.QM2.Write(ex); }
		}

		///// <summary>
		///// Deletes the user's saved layout file. Then next time will use the default file.
		///// </summary>
		//public void DeleteSavedFile() {
		//	AFile.Delete(_xmlFile);
		//}

		public FrameworkElement RootElem => _rootStack.Elem;

		public IPanel this[string name] => _aPanel.Find(p => p.Name == name);

		void _AutoUpdateXml(XElement rootStack, string xmlFileDefault) {
			var defRootStack = AExtXml.LoadElem(xmlFileDefault).Element("stack");
			var eOld = rootStack.Descendants("panel").Concat(rootStack.Descendants("toolbar")); //same speed as with .Where(cached delegate)
			var eNew = defRootStack.Descendants("panel").Concat(defRootStack.Descendants("toolbar"));
			var cmp = new _XmlNameAttrComparer();
			var added = eNew.Except(eOld, cmp);
			var removed = eOld.Except(eNew, cmp);
			if (removed.Any()) {
				foreach (var x in removed.ToArray()) {
					_Remove(x);
					AOutput.Write($"Info: {x.Name} {x.Attr("name")} has been removed in this app version.");
				}
				//removes x and ancestor stacks/tabs that would then have <= 1 child
				static void _Remove(XElement x) {
					var pa = x.Parent;
					x.Remove();
					var a = pa.Elements();
					int n = a.Count();
					if (n > 1) return;
					if (n == 1) {
						var f = a.First();
						f.SetAttributeValue("w", pa.Attr("w"));
						f.SetAttributeValue("s", pa.Attr("s"));
						f.Remove();
						pa.AddAfterSelf(f);
					}
					_Remove(pa);
				}
			}
			if (added.Any()) {
				foreach (var x in added) {
					x.SetAttributeValue("w", 50); //note: set even if was no "w", because maybe was in a tab
					rootStack.Add(x);
					AOutput.Write($"Info: {x.Name} {x.Attr("name")} has been added in this app version. You can drag and Alt+drop it in a better place.");
				}
			}
			//AOutput.Write(rootStack);
		}

		class _XmlNameAttrComparer : IEqualityComparer<XElement>
		{
			public bool Equals(XElement x, XElement y) => x.Attr("name") == y.Attr("name");
			public int GetHashCode(XElement obj) => obj.Attr("name").GetHashCode();
			//fast, same as with XName _name.
		}
	}
}
