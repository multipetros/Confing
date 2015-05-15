#region info
/* 
 * SimpleIni Class - Version 1.0
 * Copyright (C) 2015 Petros Kyladitis <http://www.multipetros.gr/>
 * 
 * Simple INI file properties manipulation class. 
 * 
 * This is free software, distributed under the therms & conditions of the FreeBSD License. For the full License text
 * see at the 'license.txt' file, distributed with this project or at <http://www.multipetros.gr/freebsd-license/>
 */
 #endregion
 
using System ;
using System.IO ;
using System.Collections.Generic ;

namespace Multipetros.Config{
	/// <summary>
	/// Simple INI file properties manipulation class. No sections or comments supported.
	/// </summary>
	public class SimpleIni : ConfigBase{
		
		private Dictionary<string, string> properties ;
		private char[] separator = new char[1] ; //1-cell char array to store separator
		private string separatorStr ;            //separator as string
		private string file ;
		private bool autosave ;
		private bool ingoreCase ;
		
		/// <summary>
		/// The simplest constructor, with filename only to configure. Autosave properties setted as False, ingore case as True, and the Separator as "=".
		/// </summary>
		/// <param name="file">A valid file path, with read and write priviliges</param>
		public SimpleIni(string file) : this(file, false, true, '='){ }
		
		/// <summary>
		/// Constructor with the filename and Autosave mode to configure. Ingore case setted True, and Separator by default as "=".
		/// </summary>
		/// <param name="file">A valid file path, with read and write priviliges.</param>
		/// <param name="autosave">True, to save each time a Property added or setted. False, to save manualy by calling the Save() method</param>
		public SimpleIni(string file, bool autosave) : this(file, autosave, true, '='){ }
		
		/// <summary>
		/// Constructor, with filename, autosave mode, ingore case, and separator to configure.
		/// </summary>
		/// <param name="file">A valid file path, with read and write priviliges.</param>
		/// <param name="autosave">True, to save each time a Property added or setted. False, to save manualy by calling the Save() method</param>
		/// <param name="ignoreCase">True to ignore case, False to make properties keys case sensitive</param>
		public SimpleIni(string file, bool autosave, bool ignoreCase) : this(file, autosave, ignoreCase, '=') { }
		
		/// <summary>
		/// The complete params constructor, with filename, autosave mode, ignore case and separator to configure
		/// </summary>
		/// <param name="file">A valid file path, with read and write priviliges.</param>
		/// <param name="autosave">True, to save each time a Property added or setted. False, to save manualy by calling the Save() method</param>
		/// <param name="ignoreCase">True to ignore case, False to make properties keys case sensitive</param>
		/// <param name="separator">Separator character for keys and values</param>
		public SimpleIni(string file, bool autosave, bool ignoreCase, char separator){
			this.file = file ;
			this.autosave = autosave ;
			this.ingoreCase = ignoreCase ;
			this.separator[0] = separator ;
			this.separatorStr = separator.ToString() ;
			Init() ;
		}
		
		/// <summary>
		/// Initialize the object by loading properties and their values from specified file 
		/// </summary>
		private void Init(){
			properties = new Dictionary<string, string>(ingoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal) ;
			if(File.Exists(file)){
				string[] lines = null ;
				try{					
					lines = File.ReadAllLines(file) ;           //load lines from file, on error throws an exception
				}catch(Exception){ }
				string[] keyval ;                               //2-cell array to store key and value of each line
				foreach(string line in lines){
					if(line.Contains(separatorStr)){            //if no separator exist, ingore this line
						keyval = line.Split(separator, 2) ;     //split line at the 1st separator apearacne
						keyval[0] = keyval[0].Trim() ;          //trim property's name from spaces
						if(properties.ContainsKey(keyval[0])){  //if key name already exist, continue to the next line
							continue ;
						}
						keyval[1] = keyval[1].Trim() ;          //trim property's value from spaces
						properties[keyval[0]] = keyval[1] ;     //add key-value pair to the Dictionary
					}
				}
			}
		}
		
		/// <summary>
		/// Store all properties, with their latest changes to the file
		/// </summary>
		public void Save(){
			string output = "" ;
			//foreach property create a new line with key-separator-value and add it to the output string
			foreach(KeyValuePair<string, string> keyval in properties){
				output += keyval.Key + separatorStr + keyval.Value + "\r\n" ;
			}
			File.WriteAllText(file, output) ;			
		}
		
		/// <summary>
		/// Access properties like Dictionary, negotiating with the Property key name eg: iniObj["keyName"]<br>
		/// When getting value of the specified property, or empty string if property not found<br>
		/// When setting a value for a non-existed property, the property will be automatically added.<br>
		/// If the setting value is <em>null</em> the property will be deleted.
		/// </summary>
		public string this[string key]{			
			get{
				if(properties.ContainsKey(key)){
					return properties[key] ;
				}else{
					//if key not found, return an empty string
					return "" ;
				}
			}
			set{
				if(value != null){
					properties[key] = value ;
				}else{
					//if setting value is null and key exist, remove this key
					if(properties.ContainsKey(key)){
						properties.Remove(key) ;
					}
				}
				if(autosave){
					Save() ;
				}
			}
		}
	}
}
