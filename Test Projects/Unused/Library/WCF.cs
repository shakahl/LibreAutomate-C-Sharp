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
using static Au.NoClass;

/* Tested WCF for IPC between editor and tasks processes.
Rejected. Now using named pipe. Reasons:

Editor		Tasks

PIPE
100 mcs, first 800 mcs
10 MB		2.8 MB
			4 threads

WCF
400 mcs, first 150 ms!!
11.5 MB		6.5 MB
			9 threads
The function is called not in main thread. Even not in same thread each time.
Loads +5 assemblies in editor, +7 in tasks process.

*/

//reference: System.ServiceModel. In editor and tasks.

//TASKS:

#if WCF //top-level code
using System.ServiceModel;

[ServiceContract]
public interface IRemoteTasks
{
	[OperationContract]
	byte Run(string csv);
}

public class RemoteTasks :IRemoteTasks
{
	public byte Run(string csv)
	{
		//Print(csv);
		return 1;
	}
}
#endif

#if WCF //in Main
		Output.LibUseQM2 = true;

		using(var host = new ServiceHost(
			  typeof(RemoteTasks),
			  new Uri[]{
		//new Uri("http://localhost:8524"),
		new Uri("net.pipe://localhost")
			  })) {
			//host.AddServiceEndpoint(typeof(IRemoteTasks),
			//  new BasicHttpBinding(),
			//  "http_RemoteTasks");

			host.AddServiceEndpoint(typeof(IRemoteTasks),
			  new NetNamedPipeBinding(),
			  "RemoteTasks");

			host.Open();

			//AuDialog.Show("Service is available.");
			WaitFor.Condition(0, () => Keyb.IsAlt);

			host.Close();
		}
		return;
#endif

//EDITOR:

#if WCF //top-level code
using System.ServiceModel;
using System.ServiceModel.Channels;

[ServiceContract]
public interface IRemoteTasks
{
	[OperationContract]
	byte Run(string csv);
}
#endif

#if WCF //in RunCompiled
byte R = _RunInAuTasksProcess_WCF(admin, csv);
#else
byte R = _RunInAuTasksProcess(admin, csv);
#endif

#if WCF //below RunCompiled
	static ChannelFactory<IRemoteTasks> s_pipeFactory;
	static IRemoteTasks s_pipeProxy;

	static byte _RunInAuTasksProcess_WCF(bool admin, string csv)
	{
		//Perf.Cpu();
		//200.ms();
		Perf.First();
		var R=_RunInAuTasksProcess_WCF2(csv);
		Perf.NW();
		Print(R);
		return 2;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static byte _RunInAuTasksProcess_WCF2(string csv)
	{
		if(s_pipeFactory == null) {
			s_pipeFactory =
		  new ChannelFactory<IRemoteTasks>(
			new NetNamedPipeBinding(),
			new EndpointAddress(
			  "net.pipe://localhost/RemoteTasks"));
		}
		if(s_pipeProxy == null) {
			s_pipeProxy =
		  s_pipeFactory.CreateChannel();
		}
		Perf.Next();

		return s_pipeProxy.Run(csv);
	}
#endif
