/* The File Commander
 * Module for UI translation into different languages
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
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
                        "FCVEVer1=Встроенный просмоторщик и редактор файлов FC {0}.\nFCVEVer2=Загружен модуль просмотра: {0}, версия {1}.{2}(C) 2013-14, разработчики FC и {3}\n" +
                        "LocalFSVer=Модуль доступа к локальным ФС [встроенный]\n" +
                        "TxtViewerVer=Просмоторщик текстовых файлов [встроенный]\n" +
                        "VEptxtVer=Простой текст\n" +
                        "FCF1=F1 Справка\n" +
                        "FCF2=F2 Меню\n" +
                        "FCF3=F3 Чтение\n" +
                        "FCF4=F4 Правка\n" +
                        "FCF5=F5 Копия\n" +
                        "FCF6=F6 Перенос\n" +
                        "FCF7=F7 Каталог\n" +
                        "FCF8=F8 Удал-е\n" +
                        "FCF9=F9 Оп-ции\n" +
                       "FCF10=F10 Выход\n" + //todo: перевести в читаемый вид
                        "FCmnuFile=&Файл\nFCmnuView=&Вид\nFCmnuNav=&Навигация\nFCmnuTools=С&ервис\nFCmnuHelp=&Справка\n" +
                        "FCmnuFileUserMenu=Меню пользователя\nFCmnuFileView=Просмотреть файл\nFCmnuFileEdit=Редактировать файл\nFCmnuFileCompare=Сравнить файлы\nFCmnuFileCopy=Копирование\nFCmnuFileMove=Перенос/переименование\nFCmnuFileNewDir=Новый каталог\nFCmnuFileRemove=Удалить\nFCmnuFileAtributes=Свойства\nFCmnuFileQuickSelect=Выбрать файл...\nFCmnuFileUnselect=Убрать синие выделения\nFCmnuFileInvertSelection=Инвертировать выделение\nFCmnuFileRevertSelection=Восстановить прежнее выделение\nFCmnuFileExit=Выxод\n" +
                        "FCmnuViewShort=Краткий (список)\nFCmnuViewDetails=Полный (таблица)\nFCmnuViewIcons=Икноки (значки)\nFCmnuViewThumbs=Эскизы изображений\nFCmnuViewQuickView=Быстрый просмотр выделенного в сосед. панели\nFCmnuViewTree=Древо каталогов\nFCmnuViewPCPCconnect=Прямая связь ПК-ПК\nFCmnuViewPCNETPCconnect=Мини HTTP сервер\nFCmnuViewByName=По имени\nFCmnuViewByType=По расширению\nFCmnuViewByDate=По дате\nFCmnuViewBySize=По размеру\nFCmnuViewNoFilter=Без фильтра (*.*)\nFCmnuViewWithFilter=Применить фильтр имени...\nFCmnuViewToolbar=Панель инструментов\n" +
                        "FCmnuNavigateTree=Дерево\nFCmnuNavigateFind=Розыск файла...\nFCmnuNavigateHistory=История посещений\nFCmnuNavigateReload=Перезагрузить\nFCmnuNavigateFlyTo=Быстрый переход\n" +
                        "FCmnuToolsOptions=Настройки...\nFCmnuToolsPluginManager=Управление расширениями...\nFCmnuToolsEditUserMenu=Редактировать меню пользователя...\nFCmnuToolsKeybrdHelp=Подсказки клавиш F\nFCmnuToolsInfobar=Сводные строки\nFCmnuToolsDiskButtons=Кнопки дисков\nFCmnuToolsConfigEdit=Правка конфигурации FC\nFCmnuToolsKeychains=Учётные данные ФС\nFCmnuToolsDiskLabel=Метка диска...\nFCmnuToolsFormat=Форматировать носитель...\nFCmnuToolsSysInfo=Сведения о системе\n" +
                        "FCmnuHelpHelpMe=Справка File Commander\nFCmnuHelpAbout=О программе...\n" +
                        "FName=Имя\nFSize=Размер\nFDate=Дата\n" +
                        "FCDelAsk=Вы действительно хотите удалить файл \"{0}\"?\n" +
                        "Canceled=Отменено пользователем\n" +
                        "Okay=ОК\n" +
                        "Cancel=Отмена\n" +
                        "FileNotFound=Файл \"{0}\" не найден\n" +
                        "ItsDir=\"{0}\" является каталогом\n" +
                        "DirCantBeRemoved=Нет возможности удалить {0}\n" +
                        "CantRunEXE=Ошибка запуска внешней программы: \n" +
                        "FileProgressDialogTitle=File Commander\n" +
                        "DoingRemove=Выполняется удаление:{0}{1}\n" +
                        "DoingCopy=Выполняется копирование:{0} В {1}{2}\n" +
                        "DoingMkdir=Создаётся каталог:{0}{1}\n" +
                        "DoingListdir=Чтение каталога:{0}{1}\n" +
                        "Directory=Каталог\n" +
                        "CopyTo=Введите путь для копирования {0}:\n" +
                        "MoveTo=Перенести/переименовать {0} в:\n" +
                        "NewDirURL=Введите путь для нового каталога\n" +
                        "NewDirTemplate=\\НОВЫЙ КАТАЛОГ\n" +
                        "FileQuickSelectFileAsk=Какой файл (маску имени) нужно выделить:\n" +
                        "FCVTitle=Просмотр - {0}\n" +
                        "FCETitle=Редактор - {0}\n" +
                        "FCVE_F1=F1 Справка\n" +
                        "FCVE_F2=F2\n" +
                        "FCVE_F3=F3\n" +
                        "FCVE_F4=F4 Вид\n" +
                        "FCVE_F5=F5 Обновить\n" +
                        "FCVE_F6=F6\n" +
                        "FCVE_F7=F7 Поиск\n" +
                        "FCVE_F8=F8 Формат\n" +
                        "FCVE_F9=F9\n" +
                       "FCVE_F10=F10 Выход\n" +
                        "FCVE_mnuFile=_Файл\nFCVE_mnuFileNew=Созд_ать\nFCVE_mnuFileOpen=_Открыть\nFCVE_mnuFileReload=Пере_загрузить\nFCVE_mnuFileSave=_Сохранить\nFCVE_mnuFileSaveAs=Со_хранить как...\n" +
                        "FCVE_mnuFilePrint=_Печать\nFCVE_mnuFilePrintPreview=Пред_варительный просмотр...\nFCVE_mnuFilePrintSettings=Пара_метры страницы...\nFCVE_mnuFileClose=В_ыход\n" +
                        "FCVE_mnuEdit=_Правка\nFCVE_mnuEditCut=_Вырезать\nFCVE_mnuEditCopy=_Копировать\nFCVE_mnuEditPaste=Вст_авить\nFCVE_mnuEditSelAll=_Выделить всё\nFCVE_mnuEditSearch=_Найти...\nFCVE_mnuEditSearchNext=Искать дальше\n" +
                        "FCVE_mnuView=_Вид\nFCVE_mnuViewModeText=_Простой текст\nFCVE_mnuViewModeImage=_Рисунок\n" +
                        "FCVE_mnuFormat=_Формат\nFCVE_mnuHelp=_Справка\nFCVE_mnuHelpHelpme=_Вызов справки\nFCVE_mnuHelpAbout=_О программе и модуле просмотра\n" +
                        "FCVWhatFind=Введите искомую строку\nFCVNothingFound=Запрошенный текст в файле не найден\n" +
                        "FCVE_mnuViewSettings=Параметры...\nFCVEpluginro1=Модуль просмотра {0} не поддерживает редактирование файлов\nFCVEpluginro2=Файл будет открыт в режиме чтения.\n" +
                        "FCVE_PluginNotFound=Не найден модуль для открытия этого файла. Файл будет выведен как текст.\nFCVE_LoadError=Ошибка загрузки файла: {0}\n" +
                        "FCS_Title=Настройки File Commander\nFCS_CantSaveSettings=Невозможно сохранить настройки. Проверьте введённые данные.\n" +
                        "swtMainWindow=Вид основного окна\nswtViewerEditor=Правка/просмотр\n" +
                        "SWTMWtoolbar=Панель инструментов\nSWTMWdiskbuttons=Кнопки дисков\nSWTMWdisklistbox=Окно выбора диска\nSWTMWpaneltitle=Заголовок файловой панели (с текущим путём)\nSWTMWtablecollumns=Заголовки столбцов\nSWTMWinfobar=Строки состояния\nSWTMWcmdline=Коммандная строка\nSWTMWkeybhelp=Подсказка клавиатуры (клавиш F)\n" +
                        "SWTVEviewer=Просмотр по F3\nSWTVEinternalv=Встроенный просмоторщик (VE)\nSWTVEexternalv=Внешний просмоторщик:\nSWTVEvesetup=Настройка встроенной программы...\nSWTVEeditor=Редактор по F4\nSWTVEinternaleditor=Встроенный редактор\nSWTVEexternaleditor=Внешний редактор:\n" +
                        "swtMainWindowColumns=    Столбцы панелей\n" +
                        "SWTMWCFileExtView=Показывать типы (расширения) файлов:\nSWTMWCExtTogether=Сразу после имени\nSWTMWCExtFar=Отдельно\nSWTMWCCollumns=Столбцы\nSWTMWCExt=Расширение\nSWTMWCSize=Pазмер\nSWTMWCDate=Изменено\nSWTMWCFileMode=Доступ\nSWTMWCExpandName=Расширить столбец \"Имя\"\nSWTMWCShowCentury=Показывать век в дате (20хх)\nSWTMWCShowTimeAs12h=12-часовой формат времени (AM/PM)\nSWTMWCShowDirsInStatus=Выводить внизу кол-во папок\nSWTMWCMaxHumanSizePan=Сокр. размер в панелях больше\nSWTMWCMaxHumanSizeStatus=Сокращать в стр. состояния >=\n";
        Dictionary<string, string> Localization = new Dictionary<string, string>();
        
        /// <summary>
        /// Получить строку с переводом
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetString(string Key){
            try{
                return Localization[Key];
            }
            catch (Exception ex) { Console.WriteLine("LOCALIZATION KEY WASN'T FOUND: " + Key + " (" + ex.Message + ")"); return Key; }
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
                catch(Exception){} //почти On Error Resume Next :-)
            }
        }
    }
}
