/* The File Commander - plugin API
 * FC & FC plugin utility set
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Xwt;
using Xwt.Drawing;
using System.Collections.Generic;
using Application = System.Windows.Forms.Application;
using Color = Xwt.Drawing.Color;
using Image = Xwt.Drawing.Image;

namespace pluginner.Toolkit
{
	public static class Utilities
	{
		static string PathToIcons = Application.StartupPath + Path.DirectorySeparatorChar + "icons";

		static Dictionary<string, Image> cached_images = new Dictionary<string, Image>();

		static Image GetLocalIcon(string path) {
			if (!cached_images.ContainsKey(path)) {
				cached_images[path] = Image.FromResource(path);
			}
			return cached_images[path];
		}

		//These functions is partially equivalents of the Xwt.MessageDialog ones
		//but they works at UI thread and in fact are macros.
		//Why C# hasn't macro-templates while the stupid C++ has?
		public static void ShowMessage(string msg1, string msg2 = null)
		{
			Xwt.Application.Invoke(delegate{
				if (msg2 == null)
					MessageDialog.ShowMessage(msg1);
				else
					MessageDialog.ShowMessage(msg1,msg2);
			});
		}

		public static void ShowError(string msg1, string msg2 = null)
		{
			Xwt.Application.Invoke(delegate{
				if (msg2 == null)
					MessageDialog.ShowError(msg1);
				else
					MessageDialog.ShowError(msg1,msg2);
			});
		}

		public static void ShowWarning(string msg1, string msg2 = null)
		{
			Xwt.Application.Invoke(delegate{
				if (msg2 == null)
					MessageDialog.ShowWarning(msg1);
				else
					MessageDialog.ShowWarning(msg1,msg2);
			});
		}

		public static int Hex2Dec(string hex)
		{
			return Convert.ToInt32(hex, 16);
		}

		/// <summary>Convert a string to a XWT color</summary>
		/// <param name="ColorName">The color name or the color hexadecimal RGB</param>
		/// <returns>Xwt.Drawing.Color that corresponds the requested colour</returns>
		public static Color GetXwtColor(string ColorName)
		{
			if (ColorName.ToLowerInvariant() == "transparent")
			{
				return Colors.Transparent;
			}

			/*string rgbhex;
			if (ColorName.StartsWith("#"))
				rgbhex = ColorName.Substring(1);
			else
				rgbhex = ColorName;
			if (rgbhex.Length != 6) return Xwt.Drawing.Color.FromName(ColorName);
			//todo: add support of rgba, hsl

			string[] colors16 = new string[3];
			colors16[0] = rgbhex.Substring(0, 1) + rgbhex.Substring(1, 1);
			colors16[1] = rgbhex.Substring(2, 1) + rgbhex.Substring(3, 1);
			colors16[2] = rgbhex.Substring(4, 1) + rgbhex.Substring(5, 1);
			int[] colors10 = new int[3];
			for (int i = 0; i < 3; i++)
			{
				colors10[i] = Hex2Dec(colors16[i]);
			}
			return new Xwt.Drawing.Color(colors10[0], colors10[1], colors10[2]);*/
			return Color.FromName(ColorName);
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

		/// <summary>Returns current XWT backend name (WPF, Gtk, Mon(o.Mac), Coc(oa) etc)</summary>
		/// <returns>WPF if WPF, Gtk if GTK, Mon if MonoMac, Coc if Cocoa</returns>
		public static string GetXwtBackendName()
		{
			return Xwt.Toolkit.CurrentEngine.GetSafeBackend (new Window()).ToString().Substring(4,3);
		}

		/// <summary>Search for icon of the selected MIME type</summary>
		/// <param name="MIME">The MIME type (i.e. application/msword)</param>
		/// <returns></returns>
		public static Image GetIconForMIME(string MIME)
		{
			if (MIME == "x-fcmd/directory")
				return GetLocalIcon("pluginner.Resources.x-fcmd-directory.png");
			
			if (MIME == "x-fcmd/up")
				return GetLocalIcon("pluginner.Resources.x-fcmd-up.png");

			if (CheckForIcon(MIME.Replace("/","-"))){
				return GetIconFromCache(MIME.Replace("/", "-"));
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
							Registry.ClassesRoot
							.OpenSubKey("."+MIME.Substring(27))
							.GetValue("")
							.ToString();

						//определение пути к иконке
						w32icon =
							Registry.ClassesRoot
							.OpenSubKey(w32type)
							.OpenSubKey("DefaultIcon")
							.GetValue("")
							.ToString();

						//извлечение иконки, сохренение в кэш и загрузка в виде XWT Image
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
						if (w32icon == null) goto mime_icon_fallback;
						Icon i =
						ExtractIconFromFile(w32icon, false);

						if (i == null) { Console.WriteLine("Extracted icon wasn't received for {0}; posssibly broken EXE/DLL or a 32/64-bit mistake", w32icon); goto mime_icon_fallback; }
						SaveIconToCache(i,MIME.Replace("/", "-"));
						return GetIconFromCache(MIME.Replace("/", "-"));
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
						Registry.ClassesRoot
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
						Registry.ClassesRoot
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
						Registry.ClassesRoot
						.OpenSubKey(Win32name)
						.OpenSubKey("DefaultIcon")
						.GetValue("")
						.ToString();
				}
				catch
				{ Console.WriteLine("Warning: No default icon is stored at HKEY_CLASSES_ROOT\\{0}\\DefaultIcon (for {1}) in the Windows registry", Win32name, MIME); }

				if (PathToIcon == null) goto mime_icon_fallback;

				//Глава 4. Извлечение иконки 16х16.
				Icon ic =
				ExtractIconFromFile(PathToIcon, false); //SEE BUG #7

				//Глава 5. Сохранение иконки в кэш и выдача готовой.
				if (ic == null) { Console.WriteLine("Extracted icon wasn't received for {0}; posssibly broken EXE/DLL or a 32/64-bit mistake", PathToIcon); goto mime_icon_fallback; }
				SaveIconToCache(ic, MIME.Replace("/", "-"));
				return GetIconFromCache(MIME.Replace("/", "-"));
			}

			//UNDONE: забубенить выдирание иконок из катАлагав /etc/mime; ~/.mime
			//TODO: сделать поддержку MacOS X.
			
			mime_icon_fallback:
#if DEBUG
			if(MIME != "application/octet-stream")
			Console.WriteLine("utilities: Can't find an icon for " + MIME);
#endif
			return GetLocalIcon("pluginner.Resources.application-octet-stream.png");
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

		#region Icon Cache utilities
		/// <summary>Save a Windows icon to cache</summary>
		/// <param name="sdi">The System.Drawing.Icon, which needs to be saved</param>
		/// <param name="name">The icon name</param>
		/// <param name="size">The icon size</param>
		public static void SaveIconToCache(Icon sdi, string name, int size = 16)
		{
			if (!Directory.Exists(PathToIcons + Path.DirectorySeparatorChar + size))
			{
				Directory.CreateDirectory(PathToIcons + Path.DirectorySeparatorChar + size);
			}

			string IconPath = PathToIcons + Path.DirectorySeparatorChar + size + Path.DirectorySeparatorChar + name + ".png";
			sdi.ToBitmap().Save(IconPath);
		}

		/// <summary>Reads an icon from icon cache</summary>
		/// <param name="name">The icon name</param>
		/// <param name="size">The icon size</param>
		/// <returns>The icon as an XWT Image.</returns>
		public static Image GetIconFromCache(string name, int size = 16)
		{
			string IconPath = PathToIcons + Path.DirectorySeparatorChar + size + Path.DirectorySeparatorChar + name + ".png";

			if(!System.IO.File.Exists(IconPath))
				return Image.FromResource("pluginner.Resources.application-octet-stream.png");
			else
				return Image.FromStream(System.IO.File.OpenRead(IconPath));
				//this way is better than Xwt.Drawing.Image.FromFile because of some GTK# issues
		}

		/// <summary>Check for existing of an icon</summary>
		/// <param name="name">The icon name</param>
		/// <param name="size">The icon preferred size</param>
		/// <returns>True or false, or there is other boolean values???</returns>
		public static bool CheckForIcon(string name, int size = 16){
			string IconPath = PathToIcons + Path.DirectorySeparatorChar + size + Path.DirectorySeparatorChar + name + ".png";
			return System.IO.File.Exists(IconPath);//todo: replace with better detection code, with support for low-res icon finding
		}
		#endregion

		#region Extractor for icons inside Win32 PE files (EXE, DLL)
		//the following code based on code from http://www.codeproject.com/Articles/29137/Get-Registered-File-Types-and-Their-Associated-Ico

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

			int commaIndex = fileAndParam.IndexOf(",", StringComparison.Ordinal);
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
		private static extern int DestroyIcon (IntPtr hIcon);
		
		/// <summary>
		/// Extract the icon from file.
		/// </summary>
		/// <param name="fileAndParam">The params string, such as ex: 
		///    "C:\\Program Files\\NetMeeting\\conf.exe,1".</param>
		/// <param name="isLarge">Determines the returned icon is a large 
		///    (may be 32x32 px) or small icon (16x16 px).</param>
		public static Icon ExtractIconFromFile(string fileAndParam, bool isLarge)
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
					Icon extractedIcon =
						(Icon)Icon.FromHandle(hIconEx[0]).Clone();

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
		#endregion

		public static bool IsMac(){
			return OSVersionEx.Platform == PlatformID.MacOSX;
		}
	}
}
