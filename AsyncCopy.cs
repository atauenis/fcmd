/* The File Commander
 * Asynchronous file copier
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace fcmd
{
	//based on the algorithm from http://krez0n.org.ua/archives/820

	/// <summary>Asynchronous file copy worker</summary>
	internal class AsyncCopy
	{
		public delegate void Complete(bool successfull);
		public delegate void Progress(string message, int percent);

		/// <summary>File copy complete</summary>
		public event Complete OnComplete;
		/// <summary>Copy progress changed</summary>
		public event Progress OnProgress;

		protected void RaiseOnComplete (bool successfull)
		{
			var handler = OnComplete;
			if (handler != null) {
				handler (successfull);
			}
		}

		/// <summary>The template of the messages about the process status</summary>
		public string ReportMessage { get; set; }

		/// <summary>Copy a file asynchronus</summary>
		/// <param name="sourceStream">The IO Stream containing the source data</param>
		/// <param name="destinationStream">The IO Stream, that should receive the data</param>
		/// <param name="BufferLenght"></param>
		public void CopyFile(System.IO.Stream sourceStream, System.IO.Stream destinationStream, int BufferLenght = 16)
		{
			if(ReportMessage==null) ReportMessage = "Copied: {0}KB / {1}KB ({2}%)";

			try
			{
				Byte[] streamBuffer = new Byte[BufferLenght];
				long totalBytesRead = 0;
				int numReads = 0;

				using (sourceStream)
				{
					long sLenght = sourceStream.Length;
					using (destinationStream)
					{
						while (true) //the loop will stop only when the EOF has been reached
						{
							numReads++;
							int bytesRead = sourceStream.Read(streamBuffer, 0, BufferLenght);

							if (bytesRead == 0){
								Report(sLenght, sLenght);
								break;
							}

							destinationStream.Write(streamBuffer, 0, bytesRead);
							totalBytesRead += bytesRead;

							//If the number of reads is a multiple of 10, send a report to UI
							if (numReads % 10 == 0){
								Report(totalBytesRead, sLenght);
							}

							//Has the loop reached the end of the file?
							if (bytesRead < BufferLenght){
								Report(totalBytesRead, sLenght);
								break;
							}
						}
					}
				}

				RaiseOnComplete(true);
			}
			catch
			{
				RaiseOnComplete(false);
				throw;
			}
		}

		/// <summary>Report the current status to the user</summary>
		/// <param name="totalBytesRead">How much is copied</param>
		/// <param name="sLenght">How much should be copied</param>
		private void Report(long totalBytesRead, long sLenght)
		{
			string message = string.Empty;
			double HowMuchDone = (double)((double)totalBytesRead / (double)sLenght);
			message = string.Format(ReportMessage,
					 totalBytesRead/1024,
					 sLenght/1024,
					 (int)(HowMuchDone * 100));

			var onProgress = OnProgress;
			if (onProgress != null && !double.IsNaN(HowMuchDone))
				onProgress(message, (int)(HowMuchDone * 100));
		}
	}
}
