/* The File Commander - API для плагинов
 * Модуль для определения подходящего для каждой цели плагина
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using fcmd.base_plugins;
using fcmd.base_plugins.viewer;

namespace fcmd
{
    class pluginfinder{
        public List<string> FSPlugins = new List<string>();
		public List<string> ViewPlugins = new List<string>();
        string[] EditPlugins; //todo

        public pluginfinder(){//конструктор
			//загрузка списка плагинов ФС из файла
			if(File.Exists(Application.StartupPath + "/fsplugins.conf")){
				string[] fsplist = File.ReadAllLines(Application.StartupPath + "/fsplugins.conf");
				int rowCounter = 0;
				foreach(string fsp in fsplist){
					rowCounter ++;
					if(fsp.Split(";".ToCharArray()).Length != 2) {Console.WriteLine("Ошибка в файле fsplugins.conf на строке " + rowCounter); break;}
					FSPlugins.Add (fsp);
				}
			}
			FSPlugins.Add("file;(internal)LocalFS"); //фсплагин по-умолчанию в конец списка

			//загрузка списка плагинов просмоторщика из файла
			if(File.Exists(Application.StartupPath + "/fcviewplugins.conf")){
				string[] vplist = File.ReadAllLines(Application.StartupPath + "/fcviewplugins.conf");
				int rowCounter = 0;
				foreach(string vp in vplist){
					rowCounter ++;
					if(vp.Split(";".ToCharArray()).Length != 3) {Console.WriteLine("Ошибка в файле fcviewplugins.conf на строке " + rowCounter); break;}
					ViewPlugins.Add (vp);
				}
			}
            ViewPlugins.Add(".*;(internal)TxtViewer;" + new Localizator().GetString("FCVViewModeText")); //зырилку по-умолчанию в конец списка

        }

        /// <summary>
        /// Fires when no plugin found for requested protocol/filetype
        /// </summary>
        [global::System.Serializable]
        public class PluginNotFoundException : Exception{
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
        public pluginner.IFSPlugin GetFSplugin(string url){
            string[] UrlParts = url.Split("://".ToCharArray());
            foreach (string CurDescription in FSPlugins){
                string[] Parts = CurDescription.Split(";".ToCharArray());
                if(System.Text.RegularExpressions.Regex.IsMatch(UrlParts[0], Parts[0])){
                    //оно!
                    if(Parts[1].StartsWith("(internal)")){//плагин встроенный
                        switch(Parts[1]){
                            case "(internal)LocalFS":
							return new fcmd.base_plugins.fs.localFileSystem();
                        }
                    }else{//плагин внешний
                        string file = Parts[1];
                        Assembly assembly = Assembly.LoadFile(file);

                        foreach (Type type in assembly.GetTypes()){
                            Type iface = type.GetInterface("pluginner.IFSPlugin");

                            if (iface != null){
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
        /// Searches for the good FCView plugin to work with the file, which starts with <paramref name="content"/> headers
        /// </summary>
		/// <param name='content'>
		/// The file full content or headers (last is better, because finding will be faster and RAM wasteless).
		/// </param>
        public pluginner.IViewerPlugin GetFCVplugin(string content){ //поиск плагина FCView
            foreach (string CurDescription in ViewPlugins){
                string[] Parts = CurDescription.Split(";".ToCharArray());
                if(System.Text.RegularExpressions.Regex.IsMatch(content, Parts[0])){
                    //оно!
                    return LoadFCVPlugin(Parts[1]);
                }
            }
			return new base_plugins.viewer.TxtViewer(); //если ничего лучшего не найти, тогда дать что имеется
        }

        /// <summary>
        /// Loads the requested viewer plugin
        /// </summary>
        /// <param name="name">The path of the plugin's DLL or internal plugin name</param>
        /// <returns>The FCView content viewing plugin</returns>
        public pluginner.IViewerPlugin LoadFCVPlugin(string name)
        {
            if(name.StartsWith("(internal)")){//плагин встроенный
                    switch(name){
                        case "(internal)TxtViewer":
                            return new base_plugins.viewer.TxtViewer();
                        case "(internal)ImgViewer":
                            //todo: return простую зырилку на базе picturebox
                            throw new PluginNotFoundException("Зырилка на базе picturebox пока что в планах"); //убрать
                    }
                }else{//плагин внешний
                    string file = name;
                    Assembly assembly = Assembly.LoadFile(file);

                    foreach (Type type in assembly.GetTypes()){
                        Type iface = type.GetInterface("pluginner.IViewerPlugin");

                        if (iface != null){
                            pluginner.IViewerPlugin plugin = (pluginner.IViewerPlugin)Activator.CreateInstance(type);
                            return plugin;
                        }
                    }    
                }

            throw new PluginNotFoundException("Search was not ended sucscessfully");
            }
        }
    }
