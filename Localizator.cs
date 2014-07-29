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
	class Localizator{
		public Localizator() {
			LoadLanguage(Settings.Default.Language);
		}

		static Dictionary<string, Dictionary<string, string>> cached_languages =
			new Dictionary<string, Dictionary<string, string>>();

		Dictionary<string, string> Localization = new Dictionary<string, string>();
		
		/// <summary>Get a string that corresponds the key in the dictionary</summary>
		/// <param name="Key">The string name (see Localizator.cs for the list of they)</param>
		public string GetString(string Key){
			if(Localization == null) throw new InvalidOperationException("The Localizator is not fed with a language file!");
			try {
				return Localization[Key];
			}
			catch (Exception ex) { Console.WriteLine(@"WARNING: Locale string is not found for key: {0} ({1})", Key, ex.Message); return Key; }
		}

		/// <summary>Load the requested localization file</summary>
		private void LoadLanguage(string url) {
			url = url.Trim();
			if (cached_languages.ContainsKey(url)) {
				Localization = cached_languages[url];
				return;
			}
			if (url.StartsWith("(internal)")) {
				switch(url)
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
				ParseLangFile(System.IO.File.ReadAllLines(url));
			}
			cached_languages[url] = Localization;
		}

		/// <summary>Load the strings form the language file body into the memory</summary>
		/// <param name="LangFile">The language file content</param>
		private void ParseLangFile(IEnumerable<string> LangFile)
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
		}
	}
}
