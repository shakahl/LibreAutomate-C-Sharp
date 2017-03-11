using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;

namespace Editor.Test
{
	//[DebuggerStepThrough]
	public static class DevTools
	{
		public static void CreatePngImagelistFileFromIconFiles_il_tv()
		{
			var a = new string[]
				{
@".cs",
@"Q:\app\folder.ico",
@"Q:\app\folder_open.ico",
				};

			_CreatePngImagelistFileFromIconFiles(a, @"Q:\app\Catkeys\Editor\Resources\il_tv.png", 16);
		}

		public static void CreatePngImagelistFileFromIconFiles_il_tb()
		{
			var a = new string[]
				{
@"Q:\app\new.ico",
@"Q:\app\properties.ico",
@"Q:\app\save.ico",
@"Q:\app\icons\run.ico",
@"Q:\app\icons\compile.ico",
@"Q:\app\deb next.ico",
@"Q:\app\icons\deb into.ico",
@"Q:\app\icons\deb out.ico",
@"Q:\app\deb cursor.ico",
@"Q:\app\deb run.ico",
@"Q:\app\deb end.ico",
@"Q:\app\undo.ico",
@"Q:\app\redo.ico",
@"Q:\app\cut.ico",
@"Q:\app\copy.ico",
@"Q:\app\paste.ico",
@"Q:\app\icons\back.ico",
@"Q:\app\icons\active_items.ico",
@"Q:\app\icons\images.ico",
@"Q:\app\icons\annotations.ico",
@"Q:\app\help.ico",
@"Q:\app\droparrow.ico",
@"Q:\app\icons\record.ico",
@"Q:\app\find.ico",
@"Q:\app\icons\mm.ico",
@"Q:\app\icons\tags.ico",
@"Q:\app\icons\resources.ico",
@"Q:\app\icons\icons.ico",
@"Q:\app\options.ico",
@"Q:\app\icons\output.ico",
@"Q:\app\tip.ico",
@"Q:\app\icons\tip_book.ico",
@"Q:\app\delete.ico",
@"Q:\app\icons\back2.ico",
@"Q:\app\open.ico",
@"Q:\app\icons\floating.ico",
@"Q:\app\icons\clone dialog.ico",
@"Q:\app\dialog.ico",
				};

			_CreatePngImagelistFileFromIconFiles(a, @"Q:\app\Catkeys\Editor\Resources\il_tb_16.png", 16);
		}

		static void _CreatePngImagelistFileFromIconFiles(string[] iconFiles, string pngFile, int imageSize)
		{
			var bAll = new Bitmap(imageSize * iconFiles.Length, imageSize);
			var g = Graphics.FromImage(bAll);
			int x = 0;

			foreach(var s in iconFiles) {
				var im = Icons.GetFileIconImage(s, imageSize);
				g.DrawImage(im, x, 0);
				x += imageSize;
			}

			bAll.Save(pngFile);
		}

		public static void CreatePngImagelistFileFromIconFiles_il_tb_big()
		{
			var a = new string[]
				{
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\VSDatasetInternalInfoFile\VSDatasetInternalInfoFile.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\ZoomIn\ZoomIn.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\XSLTTemplate\XSLTTemplate.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\XSLTTransformFile\XSLTTransformFile.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\XnaLogo\XnaLogo.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\XPath\XPath.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\XMLSchema\XMLSchema.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\XMLTransformation\XMLTransformation.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\XMLDocumentTypeDefinitionFile\XMLDocumentTypeDefinitionFile.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\XMLFile\XMLFile.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WriteBackPartition\WriteBackPartition.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFToolboxControl\WPFToolboxControl.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFUserControl\WPFUserControl.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFPageFunction\WPFPageFunction.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFResourceDictionary\WPFResourceDictionary.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFLibrary\WPFLibrary.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFPage\WPFPage.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFPage\WPFPage_gray.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFDesigner\WPFDesigner.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFFlowDocument\WPFFlowDocument.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFCustomControl\WPFCustomControl.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WorkItemQuery\WorkItemQuery.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WPFApplication\WPFApplication.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WorkflowInitiationForm\WorkflowInitiationForm.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WorkflowAssociationForm\WorkflowAssociationForm.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WorkAsSomeoneElse\WorkAsSomeoneElse.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WMIConnection\WMIConnection.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WindowsServiceStop\WindowsServiceStop.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WindowsServiceWarning\WindowsServiceWarning.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WinformToolboxControl\WinformToolboxControl.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WindowsService\WindowsService.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WindowsLogo\WindowsLogo_Cyan.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WindowsForm\WindowsForm.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WF\WF.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WFC\WFC.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WFService\WFService.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WeightMemberFormula\WeightMemberFormula.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WeightMember\WeightMember.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebUserControl\WebUserControl.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebSetupProject\WebSetupProject.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebTest\WebTest.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebService\WebService.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebPhone\WebPhone.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebPart\WebPart.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebMethodAction\WebMethodAction.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebCustomControl\WebCustomControl.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebCustomControlASCX\WebCustomControlASCX.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebConfiguration\WebConfiguration.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebConsole\WebConsole.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WebAdmin\WebAdmin.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\Web\Web.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WCFDataService\WCFDataService.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WeakHierarchy\WeakHierarchy.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\Watch\Watch.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\WCF\WCF.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\VSThemeEditor\VSThemeEditor.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\VSShell\VSShell.ico",
@"Q:\Downloads\VS2015 Image Library\2015_VSIcon\VSTAAbout\VSTAAbout.ico",
			   };

			_CreatePngImagelistFileFromIconFiles(a, @"Q:\app\Catkeys\Editor\Resources\il_tb_20.png", 20);
			_CreatePngImagelistFileFromIconFiles(a, @"Q:\app\Catkeys\Editor\Resources\il_tb_24.png", 24);
			_CreatePngImagelistFileFromIconFiles(a, @"Q:\app\Catkeys\Editor\Resources\il_tb_32.png", 32);
		}

	}
}
