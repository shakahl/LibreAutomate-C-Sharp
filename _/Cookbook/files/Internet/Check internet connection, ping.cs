/// To check Internet connection, try to connect to a known reliable Internet server. For it often is used class <see cref="Ping"/>.

using System.Net.NetworkInformation;

bool canConnectToGoogle = Internet.Ping();
print.it(canConnectToGoogle);

///
public class Internet {
	/// <summary>
	/// Sends an ICMP echo message to the specified Internet server and returns true if successful. Can be used to check Internet connectivity.
	/// </summary>
	/// <param name="hostNameOrAddress">Internet server like "google.com" or IP like "123.456.789.123".</param>
	/// <param name="timeout">Timeout in milliseconds.</param>
	/// <remarks>>
	/// Not all internet servers support it.
	/// </remarks>
	public static bool Ping(string hostNameOrAddress = "google.com", int timeout = 1000) {
		try {
			using var ping = new Ping();
			var reply = ping.Send(hostNameOrAddress, timeout);
			return reply.Status == IPStatus.Success;
		}
		catch { return false; }
	}
}
