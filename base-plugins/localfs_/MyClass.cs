using System;

namespace localfs
{
public class LocalFS : pluginner.IPlugin
 {
     public string Name
     {
         get { return "Плагин доступа к локальным ФС и сетям SMB (в Windows NT)"; }
     }

     public string Version
     {
         get { return "1.0"; }
     }

     public string Author
     {
         get { return "Александр Тауенис"; }
     }
 }
}

