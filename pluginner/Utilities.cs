/* The File Commander - plugin API
 * FC & FC plugin utility set
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace pluginner
{
	public static class Utilities
	{
		static string PathToIcons = null;

		public static int Hex2Dec(string hex)
		{
			return Convert.ToInt32(hex, 16);
		}

		public static Xwt.Drawing.Color Rgb2XwtColor(string RGBhex)
		{
			if (RGBhex.Length != 6) throw new ArgumentOutOfRangeException("RGBhex", "The color should be in the following format: HHHHHH (where H is any hexacedimal number)");
			string[] colors16 = new string[3];
			colors16[0] = RGBhex.Substring(0, 1) + RGBhex.Substring(1, 1);
			colors16[1] = RGBhex.Substring(2, 1) + RGBhex.Substring(3, 1);
			colors16[2] = RGBhex.Substring(4, 1) + RGBhex.Substring(5, 1);
			int[] colors10 = new int[3];
			for (int i = 0; i < 3; i++)
			{
				colors10[i] = Hex2Dec(colors16[i]);
			}
			return new Xwt.Drawing.Color(colors10[0], colors10[1], colors10[2]);
		}

		/// <summary>Loads embedded resource</summary>
		/// <param name="resourceName">The name of the resource</param>
		/// <param name="assembly">The assembly, which contains the resource</param>
		public static string GetEmbeddedResource(string resourceName, Assembly assembly)
		{
			resourceName = FormatResourceName(assembly, resourceName);
			using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
			{
				if (resourceStream == null)
					return null;

				using (StreamReader reader = new StreamReader(resourceStream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		private static string FormatResourceName(Assembly assembly, string resourceName)
		{
			return assembly.GetName().Name + "." + resourceName.Replace(" ", "_")
															   .Replace("\\", ".")
															   .Replace("/", ".");
		}

		/// <summary>Loads embedded resource from current program</summary>
		/// <param name="resourceName">The name of the resource</param>
		public static string GetEmbeddedResource(string resourceName)
		{
			return GetEmbeddedResource(resourceName, Assembly.GetExecutingAssembly());
		}

		/// <summary>Search for icon of the selected MIME type</summary>
		/// <param name="MIME">The MIME type (i.e. application/msword)</param>
		/// <returns></returns>
		public static Xwt.Drawing.Image GetIconForMIME(string MIME)
		{
			if (MIME == "x-fcmd/directory")
				return Xwt.Drawing.Image.FromResource("pluginner.Resources.x-fcmd-directory.png");
			
			if (MIME == "x-fcmd/up")
				return Xwt.Drawing.Image.FromResource("pluginner.Resources.x-fcmd-up.png");

			if (System.IO.File.Exists(Application.StartupPath + "/icons/" + MIME.Replace("/","-")+".png")){
				return Xwt.Drawing.Image.FromStream(System.IO.File.OpenRead(Application.StartupPath + Path.DirectorySeparatorChar + "icons" + Path.DirectorySeparatorChar + MIME.Replace("/", "-") + ".png"));
			}

			//подбор иконки по миме типу

			if (Environment.OSVersion.Platform == PlatformID.Win32NT){
				//On win32, to get the icon, it is need to go through the ass and exit from the nose
				//if you are unhappy, possibly you will exit through genitalia or ear

				/* Предисловие.
				 * На платформе Windows поиск иконки для файлов по MIME-типу без обращения к WinAPI - весёлая история.
				 * Мазохизм на примере application/msword (Word for Windows 97-2003):
				 * \\\Registry\HKEY_CLASSES_ROOT\MIME\Database\Content Type\application/msword\Extension
				 * \\\Registry\HKEY_CLASSES_ROOT\.doc\(Default)
				 * \\\Registry\HKEY_CLASSES_ROOT\Word.Document.8\DefaultIcon\(Default)
				 * Извлечь ресурс №1 из C:\\Windows\\Installer\\{90110419-6000-11D3-8CFE-0150048383C9}\\wordicon.exe
				 * Сконвертировать ICO в рисунок 16x16 px подходящего формата.
				 * 
				 * Недостаток данного метода в том, что не все расширения в Windows связаны с MIME-типом. Яркий
				 * пример - файлы Visual Studio/VC#/VB.NET. Их можно представить в виде zz-application/zz-winassoc-xxx,
				 * где ххх - расширение. Поэтому, используется второй метод определения иконки.
				 */

				//Том 1. Определение по расширению.
				if (MIME.StartsWith("zz-application/zz-winassoc"))
				{
					string w32type = null, w32icon = null;
					try
					{
						//определение типа Win32 (Word.Document.8)
						w32type =
							Microsoft.Win32.Registry.ClassesRoot
							.OpenSubKey("."+MIME.Substring(27))
							.GetValue("")
							.ToString();

						//определение пути к иконке
						w32icon =
							Microsoft.Win32.Registry.ClassesRoot
							.OpenSubKey(w32type)
							.OpenSubKey("DefaultIcon")
							.GetValue("")
							.ToString();

						//извлечение иконки, сохренение в кэш и загрузка в виде XWT Image
						if (w32icon == null) goto mime_icon_fallback;
						System.Drawing.Icon i =
						ExtractIconFromFile(w32icon, false);

						if (i == null) { Console.WriteLine("Extracted icon wasn't received for {0}; posssibly broken EXE/DLL or a 32/64-bit mistake", w32icon); goto mime_icon_fallback; }
						i.ToBitmap().Save(Application.StartupPath + Path.DirectorySeparatorChar + "icons" + Path.DirectorySeparatorChar + MIME.Replace("/", "-") + ".png");
						return Xwt.Drawing.Image.FromStream(System.IO.File.OpenRead(Application.StartupPath + Path.DirectorySeparatorChar + "icons" + Path.DirectorySeparatorChar + MIME.Replace("/", "-") + ".png"));

					}
					catch
					{ Console.WriteLine("Warning: FC is unable to get icon for .{0}", w32type); }

					if (w32type == null) goto mime_icon_fallback;
					if (w32icon == null) goto mime_icon_fallback;
				}

				//Том 2. Опеределение по MIME-типу.
				//Глава 1. Определение расширения.
				string Win32extension = null;
				try
				{
					Win32extension =
						Microsoft.Win32.Registry.ClassesRoot
						.OpenSubKey(@"MIME\Database\Content Type\" + MIME)
						.GetValue("Extension")
						.ToString();
				}
				catch
				{ Console.WriteLine("Warning: No extension associated for " + MIME + " in the Windows registry"); }

				if (Win32extension == null) goto mime_icon_fallback;

				//Глава 2. Определение Windows'овского названия типа.
				string Win32name = null;
				try
				{
					Win32name =
						Microsoft.Win32.Registry.ClassesRoot
						.OpenSubKey(Win32extension)
						.GetValue("")
						.ToString();
				}
				catch
				{ Console.WriteLine("Warning: No filetype associated at HKEY_CLASSES_ROOT\\{0} ({1}) in the Windows registry", Win32extension, MIME); }

				if (Win32name == null) goto mime_icon_fallback;

				//Глава 3. Определение ассоциированного файла-хранителя иконки.
				string PathToIcon = null;
				try
				{
					PathToIcon =
						Microsoft.Win32.Registry.ClassesRoot
						.OpenSubKey(Win32name)
						.OpenSubKey("DefaultIcon")
						.GetValue("")
						.ToString();
				}
				catch
				{ Console.WriteLine("Warning: No default icon is stored at HKEY_CLASSES_ROOT\\{0}\\DefaultIcon (for {1}) in the Windows registry", Win32name, MIME); }

				if (PathToIcon == null) goto mime_icon_fallback;

				//Глава 4. Извлечение иконки 16х16.
				System.Drawing.Icon ic =
				ExtractIconFromFile(PathToIcon, false); //SEE BUG #7

				//Глава 5. Сохранение иконки в кэш и выдача готовой.
				if (ic == null) { Console.WriteLine("Extracted icon wasn't received for {0}; posssibly broken EXE/DLL or a 32/64-bit mistake", PathToIcon); goto mime_icon_fallback; }
				ic.ToBitmap().Save(Application.StartupPath + Path.DirectorySeparatorChar + "icons" + Path.DirectorySeparatorChar + MIME.Replace("/", "-") + ".png");
				return Xwt.Drawing.Image.FromStream(System.IO.File.OpenRead(Application.StartupPath + Path.DirectorySeparatorChar + "icons" + Path.DirectorySeparatorChar + MIME.Replace("/", "-") + ".png"));
			}

			//UNDONE: забубенить выдирание иконок из катАлагав /etc/mime; ~/.mime
			//TODO: сделать поддержку MacOS X.
			
			mime_icon_fallback:
#if DEBUG
			if(MIME != "application/octet-stream")
			Console.WriteLine("utilities: Can't find an icon for " + MIME);
#endif
			return Xwt.Drawing.Image.FromResource("pluginner.Resources.application-octet-stream.png");
		}

		/// <summary>Finds a MIME content-type of a file</summary>
		/// <param name="Extension">The file's extension</param>
		/// <returns>The content type</returns>
		public static string GetContentType(string Extension)
		{
			if (Extension.Length == 0)
			{
				return "application/octet-stream";
			}

			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				//win32 way
				string CT = "application/octet-stream";

				RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(
					Extension.ToLower()
					);

				if (regKey != null){
					object contentType = regKey.GetValue("Content Type");

					if (contentType != null)
						CT = contentType.ToString();
						return CT;
				}
			}
			//todo: Linux/BSD (shared-mime-info) way and an macosx way

			return "zz-application/zz-winassoc-" + Extension;
		}

		//the following code was copypasted from http://www.codeproject.com/Articles/29137/Get-Registered-File-Types-and-Their-Associated-Ico

		/// <summary>
		/// Structure that encapsulates basic information of icon embedded in a file.
		/// </summary>
		public struct EmbeddedIconInfo
		{
			public string FileName;
			public int IconIndex;
		}

		/// <summary>
		/// Parses the parameters string to the structure of EmbeddedIconInfo.
		/// </summary>
		/// <param name="fileAndParam">The params string, such as ex: 
		///    "C:\\Program Files\\NetMeeting\\conf.exe,1".</param>
		public static EmbeddedIconInfo getEmbeddedIconInfo(string fileAndParam)
		{
			EmbeddedIconInfo embeddedIcon = new EmbeddedIconInfo();

			if (String.IsNullOrEmpty(fileAndParam))
				return embeddedIcon;

			//Use to store the file contains icon.
			string fileName = String.Empty;

			//The index of the icon in the file.
			int iconIndex = 0;
			string iconIndexString = String.Empty;

			int commaIndex = fileAndParam.IndexOf(",");
			//if fileAndParam is some thing likes this: 
				 //"C:\\Program Files\\NetMeeting\\conf.exe,1".
			if (commaIndex > 0)
			{
				fileName = fileAndParam.Substring(0, commaIndex);
				iconIndexString = fileAndParam.Substring(commaIndex + 1);
			}
			else
				fileName = fileAndParam;
    
			if (!String.IsNullOrEmpty(iconIndexString))
			{
				//Get the index of icon.
				iconIndex = int.Parse(iconIndexString);
				/*if (iconIndex < 0)
					iconIndex = 0;  //To avoid the invalid index.
				 * may cause unexpeced benaviour
				 */
			}
    
			embeddedIcon.FileName = fileName;
			embeddedIcon.IconIndex = iconIndex;

			return embeddedIcon;
		}

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern uint ExtractIconEx 
			(string szFileName, int nIconIndex, 
			IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

		[DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
		private static unsafe extern int DestroyIcon (IntPtr hIcon);
		
		/// <summary>
		/// Extract the icon from file.
		/// </summary>
		/// <param name="fileAndParam">The params string, such as ex: 
		///    "C:\\Program Files\\NetMeeting\\conf.exe,1".</param>
		/// <param name="isLarge">Determines the returned icon is a large 
		///    (may be 32x32 px) or small icon (16x16 px).</param>
		public static System.Drawing.Icon ExtractIconFromFile(string fileAndParam, bool isLarge)
		{
			unsafe
			{
				uint readIconCount = 0;
				IntPtr[] hDummy = new IntPtr[1] { IntPtr.Zero };
				IntPtr[] hIconEx = new IntPtr[1] { IntPtr.Zero };

				try
				{
					EmbeddedIconInfo embeddedIcon =
						getEmbeddedIconInfo(fileAndParam);

					if (isLarge)
						readIconCount = ExtractIconEx
						(embeddedIcon.FileName, embeddedIcon.IconIndex, hIconEx, hDummy, 1);
					else
						readIconCount = ExtractIconEx
						(embeddedIcon.FileName, embeddedIcon.IconIndex, hDummy, hIconEx, 1);

					if (readIconCount > 0 && hIconEx[0] != IntPtr.Zero)
					{
						//Get first icon.
						System.Drawing.Icon extractedIcon =
						(System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIconEx[0]).Clone();

						return extractedIcon;
					}
					else //No icon read.
						return null;
				}
				catch (Exception exc)
				{
					//Extract icon error.
					throw new ApplicationException
						("Could not extract icon", exc);
				}
				finally
				{
					//Release resources.
					foreach (IntPtr ptr in hIconEx)
						if (ptr != IntPtr.Zero)
							DestroyIcon(ptr);

					foreach (IntPtr ptr in hDummy)
						if (ptr != IntPtr.Zero)
							DestroyIcon(ptr);
				}
			}
		}
	}
}
