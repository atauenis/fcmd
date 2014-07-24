/* The File Commander
 * Module, that does the translation of the UI into some different languages
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
			LoadLanguage(fcmd.Properties.Settings.Default.Language);
		}

		Dictionary<string, string> Localization = new Dictionary<string, string>();
		
		/// <summary>Get a string that corresponds the key in the dictionary</summary>
		/// <param name="Key">The string name (see Localizator.cs for the list of they)</param>
		public string GetString(string Key){
			if(Localization == null) throw new InvalidOperationException("The Localizator is not fed with a language file!");
			try {
				return Localization[Key].Replace("{n}", "\n");
			}
			catch (Exception ex) { Console.WriteLine("WARNING: Locale string is not found for key: " + Key + " (" + ex.Message + ")"); return Key; }
		}

		/// <summary>Load the requested localization file</summary>
		private void LoadLanguage(string url){
			if (url.StartsWith("(internal)")){
				switch(url)
				{
					case "(internal)ru_RU":
						ParseLangFile(
							fcmd.Properties.Resources.lang_RusUI.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
							);
						break;
				}
			}
			else{
				ParseLangFile(System.IO.File.ReadAllLines(url));
			}
		}

		/// <summary>Load the strings form the language file body into the memory</summary>
		/// <param name="LangFile">The language file content</param>
		private void ParseLangFile(string[] LangFile)
		{
			foreach (string UIFRow in LangFile)
			{
				try
				{
					string[] Parts = UIFRow.Split('=');
					if(Parts.Length != 2) continue; //invalid rows,
					if (UIFRow.StartsWith(";") || UIFRow.StartsWith("[")) continue; // INI-section start rows and comment rows should be skipped
					Localization[Parts[0]] = Parts[1].Replace("{n}", Environment.NewLine).TrimEnd();
				}
				catch {
					Console.WriteLine(String.Format("Error during parsingc a language file, the bad string is \"{0}\"", UIFRow));
				}
			}
		}
	}
}
