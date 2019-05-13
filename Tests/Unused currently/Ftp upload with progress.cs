using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Au;
using static Au.NoClass;

static partial class Test
{
	public delegate void CbType(bool success);

	public static bool TestFtpUploadWithProgress(string localFile, string ftpURL, string ftpUser, string ftpPassword, bool autoStart, bool autoClose, int cbFunc)
	{
		var f = new FtpUploadForm();

		f.localFile = localFile;
		f.ftpURL = ftpURL;
		f.ftpUser = ftpUser;
		f.ftpPassword = ftpPassword;
		if(cbFunc != 0) f.cbFunc = (CbType)Marshal.GetDelegateForFunctionPointer((IntPtr)cbFunc, typeof(CbType));
		f.autoStart = autoStart;
		f.autoClose = autoClose;

		f.ShowDialog();

		return f.success;
	}

	class FtpUploadForm :Form
	{
		public string localFile { get; set; }
		public string ftpURL { get; set; }
		public string ftpUser { get; set; }
		public string ftpPassword { get; set; }
		public CbType cbFunc { get; set; }
		public bool autoStart { get; set; }
		public bool autoClose { get; set; }
		public bool success { get; private set; }

		Button button1;
		Label label1;
		Label label2;
		ProgressBar progressBar1;

		public FtpUploadForm()
		{
			var size = new Size(400, 240);

			button1 = new Button();
			button1.Bounds = new Rectangle(10, 10, 50, 24);
			button1.Text = "Upload";
			button1.Click += button1_Click;

			label1 = new Label();
			label1.Bounds = new Rectangle(10, 50, size.Width - 20, 50);

			label2 = new Label();
			label2.Bounds = new Rectangle(10, 100, size.Width - 20, 100);

			progressBar1 = new ProgressBar();
			progressBar1.Maximum = 100;
			progressBar1.Bounds = new Rectangle(10, 200, size.Width - 20, 20);

			this.Controls.Add(button1);
			this.Controls.Add(label1);
			this.Controls.Add(label2);
			this.Controls.Add(progressBar1);

			this.Text = "FTP Upload";
			this.ClientSize = size;
			this.StartPosition = FormStartPosition.CenterScreen;
		}

		protected override void OnLoad(EventArgs e)
		{
			if(ftpURL.Ends('/')) ftpURL += Path.GetFileName(localFile);
			label1.Text = localFile + "\n" + ftpURL;
			base.OnLoad(e);

			if(autoStart) {
				var t = new Timer(); t.Interval = 1; t.Tick += (unu, sed) => { t.Stop(); button1_Click(null, null); }; t.Start();
			}
		}

		private async void button1_Click(object sender, EventArgs e)
		{
			await processFtp();
			if(cbFunc != null) cbFunc(this.success);
			if(autoClose && this.success) this.Close();
		}

		async Task processFtp()
		{
			label2.Text = "Uploading";
			button1.Enabled = false;
			this.success = false;
			string result = "";
			try {
				FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpURL);
				request.Method = WebRequestMethods.Ftp.UploadFile;
				request.Credentials = new NetworkCredential(ftpUser, ftpPassword);

				request.KeepAlive = false;
				request.ReadWriteTimeout = 1000000000;
				request.Timeout = 1000000000;

				using(FileStream fs = File.OpenRead(localFile)) {
					using(Stream requestStream = request.GetRequestStream()) {
						byte[] b = new byte[10 * 1024];
						int readLength = 0;
						int sentLength = 0;
						while((readLength = fs.Read(b, 0, b.Length)) > 0) {
							await requestStream.WriteAsync(b, 0, b.Length);
							int percentComplete = (int)((float)(sentLength += readLength) / (float)fs.Length * 100);
							progressBar1.Value = percentComplete;
						}
					}
				}

				FtpWebResponse response = (FtpWebResponse)request.GetResponse();
				result = "Uploaded\nStatus: " + response.StatusDescription;
				response.Close();
				this.success = true;
			}
			catch(WebException e) {
				result = "Failed\n" + e.Message;
				if(e.Status == WebExceptionStatus.ProtocolError) {
					result = result + "Status Code : " + ((FtpWebResponse)e.Response).StatusCode;
					result = result + "\nStatus Description : " + ((FtpWebResponse)e.Response).StatusDescription;
				}
			}
			catch(Exception e) {
				result = e.Message;
			}
			label2.Text = result;
			button1.Enabled = true;
		}
	}

	static void TestFtpUploadSimple(string localFile, string ftpURL, string user, string password)
	{
		if(ftpURL.Ends('/')) ftpURL += Path.GetFileName(localFile);
		var request = (FtpWebRequest)WebRequest.Create(new Uri(ftpURL));
		request.Method = WebRequestMethods.Ftp.UploadFile;
		request.Credentials = new NetworkCredential(user, password);

		var bytes = File.ReadAllBytes(localFile);

		using(var requestStream = request.GetRequestStream()) {
			requestStream.Write(bytes, 0, bytes.Length);
		}

		using(var response = (FtpWebResponse)request.GetResponse()) {
			Console.WriteLine(response.StatusDescription);
		}
	}
}
