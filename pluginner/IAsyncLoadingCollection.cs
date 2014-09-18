/* The File Commander base plugins - Local filesystem adapter
 * The main part of the LocalFS FS plugin
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (С) 2014, Zhigunov Andrew (breakneck11@gmail.com)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pluginner
{
	/// <summary>
	/// Interface for objects storing a being loaded collection
	/// </summary>
	/// <typeparam name="T"></typeparam>
	interface IAsyncLoadingCollection < out T > {

		/// <summary>
		/// Get enumeratable link to the collection. Note that collection may be not fully loaded, so
		/// methods like <code>ToArray</code> or <code>ToList</code> won't be over until loading would have been done.
		/// </summary>
		IEnumerable<T> Content { get; }

		/// <summary>
		/// Get element by index. If that element hasn't been loaded yet, it will be returned after being loaded.
		/// If there is no element with such index, <value>ArgumentOutOfRangeException</value> will be thrown.
		/// </summary>
		/// <param name="index">Index of the element</param>
		/// <returns>The element with the index</returns>
		T GetByIndex(int index);

		/// <summary>
		/// Returns <value>false</value> if the whole collection was loaded (so, that's OK to use methods like <code>Content.ToArray()</code> etc.)
		/// Returns <value>true</value> otherwise.
		/// </summary>
		bool IsStillLoading { get; }

		/// <summary>
		/// If the collection is still being loaded, stop the proccess. Otherwise, do nothing.
		/// </summary>
		void Stop();
	}
}
