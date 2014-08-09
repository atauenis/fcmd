/* The File Commander
 * Module, that does the translation of the UI into some different languages
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Zhigunov Andrew (breakneck11@gmail.com)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using fcmd.Properties;

namespace fcmd
{
	public static class Localizator{
		static Localizator() {
			LoadLanguage(Settings.Default.Language);
		}

		/// <summary>
		/// Fires when the current localization file is replaced with other language file (from the Settings window)
		/// </summary>
		public static event EventHandler LocalizationChanged;
		
		static Dictionary<string, string> Localization = new Dictionary<string, string>();

		private static string CurrentDictionary;
		
		/// <summary>Get a string that corresponds the key in the dictionary</summary>
		/// <param name="Key">The string name (see Localizator.cs for the list of they)</param>
		public static string GetString(string Key){
			if(Localization == null) throw new InvalidOperationException("No localization file loaded!");
			try {
				return Localization[Key];
			}
			catch (Exception ex) { Console.WriteLine(@"WARNING: Locale string is not found for key: {0} ({1})", Key, ex.Message); return Key; }
		}

		/// <summary>Load the requested localization file</summary>
		/// <param name="URL">The URL of the localization file</param>
		/// <param name="UseCache">Enable using of cross-instace cache of current localization (set to <value>false</value> if the locale should be reloaded into the cache)</param>
		public static void LoadLanguage(string URL, bool UseCache = true) {
			URL = URL.Trim();
			if (UseCache && CurrentDictionary == URL) return; //the dictionary is already loaded
			if (URL.StartsWith("(internal)")) {
				switch(URL)
				{
					case "(internal)ru_RU":
						ParseLangFile(Resources.lang_RusUI.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
						break;
					case "(internal)en_US":
						ParseLangFile(Resources.lang_EngUI.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
						break;
				}
			}
			else {
				ParseLangFile(System.IO.File.ReadAllLines(URL));
			}
			CurrentDictionary = URL;
		}

		/// <summary>Load the strings form the language file body into the memory</summary>
		/// <param name="LangFile">The language file content</param>
		private static void ParseLangFile(IEnumerable<string> LangFile)
		{
			foreach (string UIFRow in LangFile)
			{
				try
				{
					string[] Parts = UIFRow.Split('=');
					if(Parts.Length != 2) continue; //invalid rows,
					if (!System.Text.RegularExpressions.Regex.IsMatch(UIFRow, @"^\S*=.*")) continue;
					//if (UIFRow.StartsWith(";") || UIFRow.StartsWith("[")) continue; // INI-section start rows and comment rows should be skipped
					Localization[Parts[0]] = Parts[1].Replace("{n}", Environment.NewLine).TrimEnd();
				}
				catch (Exception ex) {
					Console.WriteLine(@"An error occured when parsing the language file. The invalid string is ""{0}"". It caused an error of type {1}.", UIFRow, ex.Message);
				}
			}

			var localizationChanged = LocalizationChanged;
			if (localizationChanged != null) {
				localizationChanged (null, EventArgs.Empty);
			}
		}
	}
}
