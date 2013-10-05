/* The File Commander shared (cross-platform) kernel
 * UI localizer     Модуль для работы с переводами интерфейса на разные языки
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace fcmd
{
    class Localizator{
        public Localizator() {
            LoadUI(fcmd.Properties.Settings.Default.Language);
        }

        List<string> UIFileContent = new List<string>();
        string RusUI = "FileCommanderVer=Файловый менеджер {0}, версия {1}\n" +
                        "FCViewVer=Просмоторщик файлов, версия {0}\n" +
                        "LocalFSVer=Модуль доступа к локальным ФС [встроенный]\n"+
                        "TxtViewerVer=Просмоторщик текстовых файлов [встроенный]\n"+
                        "FCF1=F1 Справка\n" +
                        "FCF2=F2 Меню\n" +
                        "FCF3=F3 Чтение\n" +
                        "FCF4=F4 Правка\n" +
                        "FCF5=F5 Копия\n" +
                        "FCF6=F6 Перенос\n" +
                        "FCF7=F7 Каталог\n" +
                        "FCF8=F8 Удал-е\n" +
                        "FCF9=F9 Оп-ции\n" +
                       "FCF10=F10 Выход\n" +
                        "FName=Имя\nFSize=Размер\nFDate=Дата\n"+
                        "FCDelAsk=Вы действительно хотите удалить файл \"{0}\"?\n" +
                        "Canceled=Отменено пользователем\n" +
                        "FileNotFound=Файл \"{0}\" не найден\n" +
                        "ItsDir=\"{0}\" является каталогом\n" + 
                        "DirCantBeRemoved=Нет возможности удалить {0}\n" +
                        "FileProgressDialogTitle=File Commander\n"+
                        "DoingRemove=Выполняется удаление:{0}{1}\n" +
                        "DoingCopy=Выполняется копирование:{0} В {1}{2}\n" +
                        "DoingMkdir=Создаётся каталог:{0}{1}\n" +
                        "DoingListdir=Чтение каталога:{0}{1}\n" +
                        "Directory=Каталог\n" +
                        "CopyTo=Введите путь для копирования {0}:\n" +
                        "NewDirURL=Введите путь для нового каталога\n"+
                        "NewDirTemplate=\\НОВЫЙ КАТАЛОГ\n" +
                        "FCVTitle=Просмоторщик файлов - {0}\n"+
                        "FCVF1=F1 Справка\n" +
                        "FCVF2=F2\n" +
                        "FCVF3=F3\n" +
                        "FCVF4=F4 Вид\n" +
                        "FCVF5=F5 Обновить\n" +
                        "FCVF6=F6\n" +
                        "FCVF7=F7 Поиск\n" +
                        "FCVF8=F8 Формат\n" +
                        "FCVF9=F9\n" +
                       "FCVF10=F10 Выход\n" +
                        "FCVFile=&Файл\nFCVFileOpen=&Открыть\nFCVFileReload=Пере&загрузить\n" +
                        "FCVFilePrint=&Печать\nFCVFilePrintOptions=Пара&метры страницы\nFCVFileExit=З&акрыть просмотр\n" +
                        "FCVEdit=&Правка\nFCVEditCopy=&Копировать\nFCVEditSelAll=&Выделить всё\nFCVEditSearch=&Найти...\n" +
                        "FCVView=&Вид\nFCVViewModeText=&Текст\nFCVViewModeImage=&Рисунок\n" +
                        "FCVFormat=&Формат\nFCVHelpMenu=&Справка\nFCVHelpAbout=&О программе и используемом плагине";
        Dictionary<string, string> Localization = new Dictionary<string, string>();
        
        /// <summary>
        /// Получить строку с переводом
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetString(string Key){
            return Localization[Key];
        }

        /// <summary>
        /// Загрузка файла интерфейса
        /// </summary>
        /// <param name="url"></param>
        private void LoadUI(string url){
            UIFileContent.Clear();
            if (url.StartsWith("(internal)")){
                switch(url){
                    case "(internal)rus": UIFileContent.AddRange(RusUI.Split("\n".ToCharArray())); break;
                }
            }
            else{
                UIFileContent.AddRange(System.IO.File.ReadAllLines(url));
            }
            

            //парсинг файла (uifilecontent)

            foreach (string UIFRow in UIFileContent)
            {
                string[] Parts = new string[2];
                Parts = UIFRow.Split("=".ToCharArray());
                try
                {
                    Localization[Parts[0]] = Parts[1];
                }
                catch(Exception ex)
                {
                }
            }
        }
    }
}
