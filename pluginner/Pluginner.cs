/* The File Commander - plugin API
 * Base plugins' interface (common)
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;

namespace pluginner{
    public delegate string TypedEvent<T>(T data);

	/// <summary>
	/// Default plugin interface.
	/// </summary>
	public interface IPlugin{
		/// <summary>
		/// Gives the plugin's name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gives the plugin's version.
		/// </summary>
		string Version { get; }

		/// <summary>
		/// Gives the plugin's author.
		/// </summary>
		string Author { get; }
	}

	//todo: IUIPlugin (плагины к интерфейсу File Commander)

   
    #region PleaseSwitchPluginException
    /// <summary>
	/// This exception fires when the plugin module needs to be changed to an other plugin module.
	/// For example, when a filesystem plugin tried to be used with uncompatible filesystem or a image viewer plugin tried to show a text file.
    /// The File Commander should catch this exception and find a new plugin (see pluginfinder.cs file)
	/// </summary>
	[System.Serializable]
	public class PleaseSwitchPluginException : System.Exception
	{
		/// <summary>
		/// Informs the File Commander that the plugin cannot be used now and must be changed
		/// </summary>
		public PleaseSwitchPluginException ()
		{
		}
		
		/// <summary>
		/// Informs the File Commander that the plugin cannot be used now and must be changed
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception reason. </param>
		public PleaseSwitchPluginException (string message) : base (message)
		{
		}
		
		/// <summary>
		/// Informs the File Commander that the plugin cannot be used now and must be changed. The reason should be showed in the <see cref="inner"/>.
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public PleaseSwitchPluginException (string message, System.Exception inner) : base (message, inner)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:PleaseSwitchPluginException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected PleaseSwitchPluginException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
        }
    #endregion
    }
}

