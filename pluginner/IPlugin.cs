/* The File Commander - plugin API
 * Base plugins' interface (common)
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Configuration;
using System.Runtime.Serialization;

namespace pluginner{
	public delegate void TypedEvent<T>(T data);
	public delegate void TypedEvent<T1, T2>(T1 sender, T2 data);

	/// <summary>
	/// Default plugin interface.
	/// </summary>
	public interface IPlugin{
		//BASIC PLUGIN'S PROPERTIES
		//ОСНОВНЫЕ СВОЙСТВА ПЛАГИНОВ
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

		//FLEXIBLE APPLICATION PROGRAMMING INTERFACE
		//ГИБКОЕ API
		/// <summary>
		/// (int[6]) Defines mimimal and maximal versions of the flexible API.
		/// Currently it is {0,1,0, 0,1,0}
		/// </summary>
		int[] APICompatibility { get; }

		/// <summary>
		/// Talk something to the plugin (to be called by the host)
		/// </summary>
		/// <param name="call">The function name</param>
		/// <param name="arguments">The argument or arguments</param>
		/// <returns>Something, that the function returns (the type is defined in the FC flexible API documentation)</returns>
		object APICallPlugin(string call, params object[] arguments);

		/// <summary>
		/// This event should be raised to talk something to the File Commander (to be raised by the plugin)
		/// </summary>
		event TypedEvent<object[]> APICallHost;

		/// <summary>
		/// This property is used to access the current FC User Settings from the Plugin
		/// </summary>
		Configuration FCConfig { set; }
	}

	//todo: IUIPlugin (плагины к интерфейсу File Commander), IArchPlugin (архивные плагины)

   
	#region PleaseSwitchPluginException
	/// <summary>
	/// This exception fires when the plugin module needs to be changed to an other plugin module.
	/// For example, when a filesystem plugin tried to be used with uncompatible filesystem or a image viewer plugin tried to show a text file.
	/// The File Commander should catch this exception and find a new plugin (see pluginfinder.cs file)
	/// </summary>
	[Serializable]
	public class PleaseSwitchPluginException : Exception
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
		public PleaseSwitchPluginException (string message, Exception inner) : base (message, inner)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:PleaseSwitchPluginException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected PleaseSwitchPluginException (SerializationInfo info, StreamingContext context) : base (info, context)
		{
		}
	#endregion
	}
}

