/* The File Commander
 * Client for File Transfer Protocol (FTP)
 * Implements RFC 959 (10/1985)
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace fcmd
{
	/// <summary>
	/// A client for File Transfer Protocol
	/// </summary>
	public class FTPClient
	{
		/// <summary>
		/// Represents any FTP error
		/// </summary>
		public class FtpException : Exception
		{
			public FtpException(string Message, int Code, string Command) : base("Got non-good FTP response " + Code + ": " + Message) { }
			public FtpException(string Message, Exception InnerException) : base(Message, InnerException) { }
		}

		private Socket CommandSocket;
		private Byte[] Buffer = new byte[512];

		public int ResponseCode;
		public string Response;

		public FTPClient(string Server, int Port = 21, string Username = "anonymous", string Password = @"anonymous@filecommander.org")
		{
			PasvMode = true;

			Console.WriteLine(@"FTP: Connecting to {0}...",Server);

			IPAddress addr;

			try
			{
				CommandSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				addr = Dns.GetHostEntry(Server).AddressList[0];
				CommandSocket.Connect(addr,Port);
			}
			catch (Exception ex)
			{
				if (CommandSocket != null && CommandSocket.Connected) CommandSocket.Close();

				throw new FtpException("Couldn't connect to remote server", ex);
			}

			Console.WriteLine(@"FTP: TCP connection estabilished " + Server);
			ReadResponse();

			if (ResponseCode != 220)
			{
				//close
				throw new FtpException(Response.Substring(4),ResponseCode,"connnect");
			}

			Console.WriteLine(@"FTP: Authorizing...");

			SendCommand("USER " + Username);

			if (!(ResponseCode == 331 || ResponseCode == 230 || ResponseCode == 220))
			{
				//close
				throw new FtpException(Response.Substring(4),ResponseCode,"USER");
			}

			if (ResponseCode != 230)
			{
				SendCommand("PASS " + Password);

				if (!(ResponseCode == 230 || ResponseCode == 202))
				{
					//close
					throw new FtpException(Response.Substring(4),ResponseCode,"PASS");
				}
			}

			Console.WriteLine(@"FTP: FTP connection estabilished " + Server);
		}

		/// <summary>
		/// Gets the network socket for data I/O operation
		/// </summary>
		public Socket GetDataSocket()
		{
			if (PasvMode)
			{
				//passive mode
				SendCommand("PASV");

				if (ResponseCode != 227) throw new FtpException(Response.Substring(4), ResponseCode,"PASV");

				int index1 = Response.IndexOf('(');
				int index2 = Response.IndexOf(')');

				string ipData = Response.Substring(index1 + 1, index2 - index1 - 1);

				int[] parts = new int[6];

				int len = ipData.Length;
				int partCount = 0;
				string buf = "";

				for (int i = 0; i < len && partCount <= 6; i++)
				{
					char ch = char.Parse(ipData.Substring(i, 1));

					if (char.IsDigit(ch))
						buf += ch;

					else if (ch != ',')
						throw new FtpException("Malformed PASV response: " + Response, ResponseCode,"PASV");

					if (ch == ',' || i + 1 == len)
					{
						try
						{
							parts[partCount++] = int.Parse(buf);
							buf = "";
						}
						catch (Exception ex)
						{
							throw new FtpException("Malformed PASV response (not supported?): " + Response, ex);
						}
					}
				}

				string ipAddress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];

				int port = (parts[4] << 8) + parts[5];

				Socket socket = null;

				try
				{
					Console.WriteLine(@"FTP: Opening passive data connection to {0}:{1}",ipAddress,port);
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					IPEndPoint ep = new IPEndPoint(Dns.GetHostEntry(ipAddress).AddressList[0], port);
					socket.Connect(ep);
				}
				catch (Exception ex)
				{
					Console.WriteLine(@"FTP: Unable to open data connection: " + ex.Message);
					if (socket != null && socket.Connected) socket.Close();

					throw new FtpException("Can't connect to remote server", ex);
				}
				Console.WriteLine(@"FTP: ready to exchange data with {0}:{1}", ipAddress, port);
				return socket;
			}
			else
			{
				//PORT mode
				//todo
				throw new NotImplementedException("Active (PORT) mode is not implemented");
			}
		}

		/// <summary>
		/// Gets or sets the data transfer host and port
		/// </summary>
		public IPEndPoint PORT { get; set; }
		
		/// <summary>
		/// Gets or sets the data transfer mode (<value>true</value>=passive, <value>false</value>=port based). For details, see http://tools.ietf.org/html/rfc959.
		/// </summary>
		public bool PasvMode { get; set; }
		
		/// <summary>
		/// Gets server's responce at port 21
		/// </summary>
		public void ReadResponse()
		{
			Response = "";
			ResponseCode = 0;
			Buffer = new byte[512];

			if (CommandSocket.Connected)
			{
				do {} while (CommandSocket.Available < 1); //wait while data is receiving

				CommandSocket.Receive(Buffer, Buffer.Length, 0);
				Response = Encoding.UTF8.GetString(Buffer);
				if (Response.Contains("\n"))
				{ //server sent many responses in the packet, store only last
					string[] responces = Response.Split('\n');
					Response = responces[responces.Length - 2];
					ResponseCode = int.Parse(Response.Substring(0, 3));
					Console.WriteLine(@"FTP: < {0}", Response);
				}
				else
				{
					ResponseCode = int.Parse(Response.Substring(0, 3));
					Console.WriteLine(@"FTP: < {0} [SINGLE]", Response);
				}
			}
			else throw new Exception("Command socket has been closed");
		}

		/// <summary>
		/// Sends a FTP command and returns its response
		/// </summary>
		/// <param name="Command">The command with arguments (no line endigng)</param>
		/// <param name="Wait">Wait for response begin received</param>
		/// <returns>The server response code</returns>
		public int SendCommand(string Command, bool Wait = true)
		{
			Console.WriteLine(@"FTP: > {0}", Command);
			Byte[] cmdBytes = Encoding.ASCII.GetBytes((Command + "\r\n").ToCharArray());
			try
			{
				CommandSocket.Send(cmdBytes, 0, cmdBytes.Length, SocketFlags.None);
			}
			catch (Exception ex)
			{
				throw new FtpException("Network error: " + ex.Message,ex);
			}

			if (Wait)
			{
				ReadResponse();
				return ResponseCode;
			}
			else return -1;
		}

		/// <summary>
		/// Sends a FTP command and returns its response
		/// </summary>
		/// <param name="Command">The command with arguments (no line endigng)</param>
		/// <param name="Result">The server's response (from command socket)</param>
		/// <returns>The server response code</returns>
		public int SendCommand(string Command, out string Result)
		{
			SendCommand(Command);
			Result = Response;
			return ResponseCode;
		}

		/// <summary>
		/// Sends a FTP command and returns its response
		/// </summary>
		/// <param name="Command">The command with arguments (no line endigng)</param>
		/// <param name="Result">The server's response (from command socket)</param>
		/// <param name="GoodCodes">List (array) of response codes that should be interpretted as "all okay", others will cause throwing of an FtpException</param>
		/// <returns>The server response code</returns>
		public int SendCommand(string Command, out string Result, IEnumerable<int> GoodCodes)
		{
			int result = SendCommand(Command, out Result);
			if (GoodCodes.Any(i => result == i))
				return result;
			throw new FtpException(Response,ResponseCode,Command);
		}
		/// <summary>
		/// Sends a FTP command and returns its response
		/// </summary>
		/// <param name="Command">The command with arguments (no line endigng)</param>
		/// <param name="Result">The server's response (from command socket)</param>
		/// <param name="GoodCode">The response codes that should be interpretted as "all okay", others will cause throwing of an FtpException</param>
		/// <returns>The server response code</returns>

		public int SendCommand(string Command, out string Result, int GoodCode)
		{
			return SendCommand(Command, out Result, new int[] {GoodCode});
		}


		~FTPClient()
		{
			if (CommandSocket != null)
			{
				if(CommandSocket.Connected)
					CommandSocket.Close();

				CommandSocket = null;
			}
		}
	}
}
