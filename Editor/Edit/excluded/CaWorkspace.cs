using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Compiler;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.CSharp;

class CaWorkspace : Workspace
{
	//public CaWorkspace(HostServices host) : base(host, WorkspaceKind.Host)
	public CaWorkspace() : base(MefHostServices.DefaultHost, WorkspaceKind.Host) //TODO: Roslyn throws/handles 2 exceptions because does not find Workspaces and Features assemblies for VB
	{

	}

	//private static MefHostServices CreateHostServices()
	//{
	//	var compositionHost = new ContainerConfiguration().WithAssemblies(MefHostServices.DefaultAssemblies).CreateContainer();
	//	return MefHostServices.Create(compositionHost);
	//}

	public override bool CanOpenDocuments => true;

	public override bool CanApplyChange(ApplyChangesKind feature)
	{
		switch(feature) {
		case ApplyChangesKind.ChangeDocument:
		case ApplyChangesKind.ChangeDocumentInfo:
		case ApplyChangesKind.AddMetadataReference:
		case ApplyChangesKind.RemoveMetadataReference:
			return true;
		}
		return false;
	}

	protected override void ApplyDocumentTextChanged(DocumentId documentId, SourceText text)
	{
		OnDocumentTextChanged(documentId, text, PreservationMode.PreserveValue);
	}

	//protected override string GetDocumentName(DocumentId documentId)
	//{
	//	Print(documentId);
	//	var r=base.GetDocumentName(documentId);
	//	Print(r, r != null);
	//	return r;
	//}

	public bool AddProject(FileNode f)
	{
		var m = new MetaComments();
		IWorkspaceFile projFolder = null;//TODO
		if(!m.Parse(f, projFolder, EMPFlags.ForCodeInfo)) return false;

		var projectId = ProjectId.CreateNewId();
		var name = f.Name;
		var references = m.References.Refs;

		var projectInfo = ProjectInfo.Create(projectId, VersionStamp.Default, name, name, LanguageNames.CSharp,
			metadataReferences: references,
			parseOptions: new CSharpParseOptions(LanguageVersion.Preview));
		OnProjectAdded(projectInfo);

		var documentId = DocumentId.CreateNewId(projectId);
		var documentInfo = DocumentInfo.Create(documentId, name,
							loader: TextLoader.From(TextAndVersion.Create(SourceText.From(f.GetText(), Encoding.UTF8), VersionStamp.Create())));
		OnDocumentAdded(documentInfo);

		f.CaDocumentId = documentId;
		return true;
	}

	public void RemoveProject(FileNode f)
	{
		OnProjectRemoved(f.CaDocumentId.ProjectId);
	}

	public void UpdateText(FileNode f, string text)
	{
		OnDocumentTextChanged(f.CaDocumentId, SourceText.From(text, Encoding.UTF8), PreservationMode.PreserveValue);
	}
}
