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
using fcmd.base_plugins;
using fcmd.base_plugins.viewer;

namespace fcmd
{
    class pluginfinder{
        string[] FSPlugins = new string[1];
		List<string> ViewPlugins = new List<string>();
        string[] EditPlugins;

        public pluginfinder(){//конструктор
            FSPlugins[0] = "file;(internal)LocalFS";//протокол file://
            //ViewPlugins[0] = ".*;(internal)TxtViewer";//маска "усё"
            //todo: чтение из файла настроек

			//загрузка списка плагинов из файла
			if(File.Exists(Application.StartupPath + "/fcviewplugins.conf")){
				string[] vplist = File.ReadAllLines(Application.StartupPath + "/fcviewplugins.conf");
				int rowCounter = 0;
				foreach(string vp in vplist){
					rowCounter ++;
					if(vp.Split(";".ToCharArray()).Length != 2) {Console.WriteLine("Ошибка в файле fcviewplugins.conf на строке " + rowCounter); break;}
					ViewPlugins.Add (vp);
				}
			}
			ViewPlugins.Add(".*;(internal)TxtViewer"); //зырилку по-умолчанию в конец списка

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
        public pluginner.IFSPlugin GetFSplugin(string url){ //todo!!!
            string[] Parts = url.Split("://".ToCharArray());
            if (Parts[0] == "file")
            {
                return new localFileSystem();
                //todo: определение
            }
            else
            {
                throw new PluginNotFoundException("Плагин для ФС " + Parts[0] + " не найден");
            }
        }

        public pluginner.IViewerPlugin GetFCVplugin(string content){
            foreach (string CurDescription in ViewPlugins){
                string[] Parts = CurDescription.Split(";".ToCharArray());
                if(System.Text.RegularExpressions.Regex.IsMatch(content, Parts[0])){
                    //оно!
                    if(Parts[1].StartsWith("(internal)")){//плагин встроенный
                        switch(Parts[1]){
                            case "(internal)TxtViewer":
                                return new base_plugins.viewer.TxtViewer();
                            case "(internal)ImgViewer":
                                //todo: return простую зырилку на базе picturebox
                                throw new PluginNotFoundException("Зырилка на базе picturebox пока что в планах"); //убрать
                        }
                    }else{//плагин внешний
                        //todo: добавить загрузку плагинов
                    }
                }
            }
			return new base_plugins.viewer.TxtViewer(); //если ничего лучшего не найти, тогда дать что имеется
        }
    }
}
