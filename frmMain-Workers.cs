/* The File Commander - главное окно
 * Таджики для работ с файлами в фоновых потоках
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace fcmd{
    public partial class frmMain{
        /* ЗАМЕТКА РАЗРАБОТЧИКУ
         * 
         * В данном файле размещаются функции для работы с файлами и каталогами.
         * Данные функции работают в отдельных от UI потоках и вызваются функциями
         * из файлов frmMain.cs и frmMain-CLI.cs .
         */

        /// <summary>
        /// Background directory lister
        /// </summary>
        void DoLs(string URL, ListPanel lp){
            lp.Items.Clear();

            //гружу директорию
            pluginner.IFSPlugin fsp = lp.FSProvider;
            fsp.CurrentDirectory = URL;
            foreach (pluginner.DirItem di in fsp.DirectoryContent)
            { //перебираю файлы, найденные провайдером ФС
                if (di.Hidden == false)
                {
                    ListPanel.ItemDescription NewItem;
                    NewItem = new ListPanel.ItemDescription();
                    NewItem.Text.Add(di.TextToShow);
                    NewItem.Text.Add(Convert.ToString(di.Size / 1024) + "KB");
                    NewItem.Text.Add(di.Date.ToShortDateString());
                    NewItem.Value = di.Path;
                    NewItem.Selection = 0;
                    NewItem.Selected = false;
                    lp.Items.Add(NewItem);
                }
            }
        }

        /// <summary>
        /// Background file copier
        /// </summary>
        /// <param name="lpa">active listpanel</param>
        /// <param name="lpp">passive listpanel</param>
        void DoCp(ListPanel lpa, ListPanel lpp, string DestinationURL, pluginner.File SourceFile){
            pluginner.IFSPlugin DestinationFS = lpp.FSProvider;

            pluginner.File NewFile = SourceFile;
            NewFile.Path = DestinationURL;

            DestinationFS.WriteFile(NewFile);
        }
    }
}
