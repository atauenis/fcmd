/* The File Commander - plugin API
 * The plugin finder
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using fcmd.base_plugins;
using fcmd.base_plugins.fs;

namespace fcmd
{
	class pluginfinder
	{
		public List<string> FSPlugins = new List<string>();
		public List<string> VEPlugins = new List<string>();

		public pluginfinder()
		{//конструктор
			//загрузка списка плагинов ФС из файла
			if (File.Exists(Environment.CurrentDirectory + "/fsplugins.conf"))
			{
				string[] fsplist = File.ReadAllLines(Environment.CurrentDirectory + "/fsplugins.conf");
				int rowCounter = 0;
				foreach (string fsp in fsplist)
				{
					rowCounter++;
					if (fsp.Split(";".ToCharArray()).Length != 3) { Console.WriteLine("Ошибка в файле fsplugins.conf на строке " + rowCounter); break; }
					FSPlugins.Add(fsp);
				}
			}
			FSPlugins.Add("ftp;(internal)FTPFS;FTP");
			FSPlugins.Add("file;(internal)LocalFS;" + Localizator.GetString("LocalFSVer"));

			//load the list of VE plugins
			if (File.Exists(Environment.CurrentDirectory + "/veplugins.conf"))
			{
				string[] vplist = File.ReadAllLines(Environment.CurrentDirectory + "/veplugins.conf");
				int rowCounter = 0;
				foreach (string vp in vplist)
				{
					rowCounter++;
					if (vp.Split(";".ToCharArray()).Length != 3) { Console.WriteLine("Error in veplugins.conf at row " + rowCounter); break; }
					VEPlugins.Add(vp);
				}
			}
			VEPlugins.Add(
			@"<?xml ;(internal)PlainXml;XML\n"+
			".*;(internal)PlainText;" + Localizator.GetString("FCVViewModeText")); //зырилки по-умолчанию в конец списка

		}

		/// <summary>
		/// Fires when no plugin found for requested protocol/filetype
		/// </summary>
		[global::System.Serializable]
		public class PluginNotFoundException : Exception
		{
			public PluginNotFoundException() { }
			public PluginNotFoundException(string message) : base(message) { }
			public PluginNotFoundException(string message, Exception inner) : base(message, inner) { }
			protected PluginNotFoundException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context)
				: base(info, context) { }
		}

		/// <summary>
		/// Searches for the good FS plugin to work with filesystem of the file/directory <paramref name="url"/>
		/// </summary>
		/// <param name="url">The uniform resource locator for the requested file</param>
		/// <returns>The good filesystem plugin (IFSPlugin-based class) or an exception if no plugins found</returns>
		public pluginner.IFSPlugin GetFSplugin(string url)
		{
			string[] UrlParts = url.Split("://".ToCharArray());
			foreach (string CurDescription in FSPlugins)
			{
				string[] Parts = CurDescription.Split(";".ToCharArray());
				if (System.Text.RegularExpressions.Regex.IsMatch(UrlParts[0], Parts[0]))
				{
					//оно!

					System.Configuration.Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal);

					if (Parts[1].StartsWith("(internal)"))
					{//плагин встроенный
						switch (Parts[1])
						{
							case "(internal)LocalFS":
								return new fcmd.base_plugins.fs.localFileSystem { FCConfig = conf };
							case "(internal)FTPFS":
								return new FTPFileSystem {FCConfig = conf};
							default:
								throw new PluginNotFoundException("The filesystem plugin " + Parts[1] + " is not embedded into FC Commander");
						}
					}
					else
					{//плагин внешний
						string file = Parts[1];
						Assembly assembly = Assembly.LoadFile(file);

						foreach (Type type in assembly.GetTypes())
						{
							Type iface = type.GetInterface("pluginner.IFSPlugin");

							if (iface != null)
							{
								pluginner.IFSPlugin plugin = (pluginner.IFSPlugin)Activator.CreateInstance(type);
								return plugin;
							}
						}
					}
				}
			}
			throw new PluginNotFoundException("Не найден плагин ФС для протокола " + UrlParts[0]);
		}

		/// <summary>
		/// Search for VE plugin
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public pluginner.IVEPlugin GetFCVEplugin(string content)
		{ //поиск плагина FC VE
			foreach (string CurDescription in VEPlugins)
			{
				string[] Parts = CurDescription.Split(";".ToCharArray());
				if (System.Text.RegularExpressions.Regex.IsMatch(content, Parts[0]))
				{
					//оно!
					return LoadFCVEPlugin(Parts[1]);
				}
			}
			return new base_plugins.ve.PlainText(); //если ничего лучшего не найти, тогда дать что имеется
		}

		public pluginner.IVEPlugin LoadFCVEPlugin(string name)
		{
			if (name.StartsWith("(internal)"))
			{//плагин встроенный
				switch (name)
				{
					case "(internal)PlainText":
						return new base_plugins.ve.PlainText();
					case "(internal)PlainXml":
						return new base_plugins.ve.PlainXml();
					/* ==INTERNAL PLUGINS, THAT NEEDS TO BE CREATED==
					 * a simple raster image viewer/editor based on xwt drawing possibilities
					 * a markdown viewer based on Xwt.MarkdownView (readonly)
					 * a HEXadecimal editor (maybe integrated into PlainText as editor)
					 * a csv table viewer/editor
					 * a html viewer (based on xwt's webview)
					 */
				}
			}
			else
			{//плагин внешний
				string file = name;
				Assembly assembly = Assembly.LoadFile(file);

				foreach (Type type in assembly.GetTypes())
				{
					Type iface = type.GetInterface("pluginner.IVEPlugin");

					if (iface != null)
					{
						pluginner.IVEPlugin plugin = (pluginner.IVEPlugin)Activator.CreateInstance(type);
						return plugin;
					}
				}
			}

			throw new PluginNotFoundException("Cannot load VE plugin " + name + " because it is somewhere else, but not in known places.");
		}
	}

	}
