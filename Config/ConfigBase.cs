#region info
/* 
 * ConfigBase Class - Version 1.0
 * Copyright (C) 2015 Petros Kyladitis <http://www.multipetros.gr/>
 * 
 * Base class with static methods for easily configuration properties manipulation. 
 * 
 * This is free software, distributed under the therms & conditions of the FreeBSD License. For the full License text
 * see at the 'license.txt' file, distributed with this project or at <http://www.multipetros.gr/freebsd-license/>
 */
 #endregion
 
using System ;

namespace Multipetros.Config{	
	/// <summary>
	/// Provide static methods for easily configuration properties manipulation, such as base64 encoding, add & remove double quotes, etc.
	/// </summary>
	public class ConfigBase{
		public ConfigBase(){ }
		
		/// <summary>
		/// Encode unicode text to Base64
		/// </summary>
		/// <param name="val">Text to encode</param>
		/// <returns>Encoded text</returns>
		public static string EncodeB64(string val){
			return Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(val)) ;
		}
		
		/// <summary>
		/// Decode text from Base64 to Unicode
		/// </summary>
		/// <param name="val">Text to decode</param>
		/// <returns>Decoded text</returns>
		public static string DecodeB64(string val){
			return System.Text.Encoding.Unicode.GetString(Convert.FromBase64String(val)) ;
		}
		
		/// <summary>
		/// Add double quotes to the text. You can use this if you want to store properites values with spaces at the begin and/or the end
		/// </summary>
		/// <param name="val">Text to add double quotes</param>
		/// <returns></returns>
		public static string AddQuotes(string val){
			return "\"" + val + "\"" ;
		}
		
		/// <summary>
		/// Remove double quotes from the text. This is for strip double quotes from properties values stored in this way.
		/// </summary>
		/// <param name="val">Text to trim double quotes</param>
		/// <returns></returns>
		public static string RemoveQuotes(string val){
			return val.Trim("\"".ToCharArray()) ;
		}
	}
}
