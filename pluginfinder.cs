/* The File Commander - API для плагинов
 * Модуль для определения подходящего для каждой цели плагина
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.Text;
using fcmd.base_plugins;
using fcmd.base_plugins.viewer;

namespace fcmd
{
    class pluginfinder{
        string[] FSPlugins = new string[1];
        string[] ViewPlugins = new string[1];
        string[] EditPlugins;

        public pluginfinder(){//конструктор
            FSPlugins[0] = "file;(internal)LocalFS";//протокол file://
            ViewPlugins[0] = ".*;(internal)TxtViewer";//маска "усё"
            //todo: чтение из файла настроек
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
            System.Windows.Forms.MessageBox.Show(Parts[0]);//убрать
            if (Parts[0] == "file")
            {
                return new localFileSystem();
                //todo: определение
            }
            else
            {
                throw new PluginNotFoundException("Плагин для типа " + Parts[0] + " не найден");
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
                                return new TxtViewer();
                            case "(internal)ImgViewer":
                                //todo: return простую зырилку на базе picturebox
                                throw new PluginNotFoundException("Зырилка на базе picturebox пока что в планах"); //убрать
                        }
                    }else{//плагин внешний
                        //todo: добавить загрузку плагинов
                    }
                }
            }
            throw new PluginNotFoundException("Не найден плагин просмотра");
        }
    }
}
