#region info
/* 
 * RegistryIni Class - Version 1.0
 * Copyright (C) 2015 Petros Kyladitis <http://www.multipetros.gr/>
 * 
 * Configuration properties manipulation at the MS Windows Registry
 * 
 * This is free software, distributed under the therms & conditions of the FreeBSD License. For the full License text
 * see at the 'license.txt' file, distributed with this project or at <http://www.multipetros.gr/freebsd-license/>
 */
 #endregion

using System ;
using Microsoft.Win32 ;

namespace Multipetros.Config{
	
	/// <summary>
	/// 
	/// </summary>
	public enum RegistryNode{
		CurrentUser,
		LocalMachine
	}
	
	/// <summary>
	/// Μanipulate configuration properties, at the MS Windows Registry
	/// </summary>
	public class RegistryIni : ConfigBase{
		private RegistryKey baseAppKey ;                   // The object to contact with the current instance reg subkeys
		private string companyName ;                       // Company's name, to use it as the parent reg subkey
		private string productName ;                       // Product's name, use it as child of company's name key, or parent if company is null
		private RegistryNode regNode ;                     // Store struct value, to select the saving registry node
		private char[] keySeparator = new char[1]{'\\'} ;  // A virtual separator for registry keys in a string
		private string[] emptyStrArray = new string[0] ;   // An empty array, to help overriding indexer method calls
		
		
		/// <summary>
		/// The simplest constructor, with only the product name to configure. The properties stored under the HKEY_CURRENT_USER\Software\productName\ node
		/// </summary>
		/// <param name="productName">The product name. Used as parent key.</param>
		public RegistryIni(string productName) : this(null, productName, RegistryNode.CurrentUser){ }
		
		/// <summary>
		/// Constructor with company & product name to configure. The properties stored under the HKEY_CURRENT_USER\Software\companyName\productName\ node
		/// </summary>
		/// <param name="companyName">The company name. Used as parent key.</param>
		/// <param name="productName">The product name. Used as child key of the company's name.</param>
		public RegistryIni(string companyName, string productName) : this(companyName, productName, RegistryNode.CurrentUser){ }
		
		/// <summary>
		/// Constructor with product name & registry node to configure. The properties can be stored under the: <ul>
		/// <li> HKEY_CURRENT_USER\Software\productName\ node, or </li>
		/// <li> HKEY_LOCAL_MACHINE\Software\productName\ node, or </li>
		/// <li> HKEY_LOCAL_MACHINE\Software\WOW6432Node\productName\ node for 64bit setups, when 32bit binary used. </li></ul>
		/// </summary>
		/// <param name="productName">The product name. Used as parent key.</param>
		/// <param name="regNode">Registy Node (HKEY_CURRENT_USER or HKEY_LOCAL_MACHINE) selection to use.</param>
		public RegistryIni(string productName, RegistryNode regNode) : this(null, productName, regNode){ }		
		
		/// <summary>
		/// Constructor with company name, product name & registry node to configure. The properties can be stored under the: <ul>
		/// <li> HKEY_CURRENT_USER\Software\companyName\productName\ node, or </li>
		/// <li> HKEY_LOCAL_MACHINE\Software\companyName\productName\ node, or </li>
		/// <li> HKEY_LOCAL_MACHINE\Software\WOW6432Node\companyName\productName\ node for 64bit setups, when 32bit binary used. </li></ul>
		/// </summary>
		/// <param name="companyName">The company name. Used as parent key.</param>
		/// <param name="productName">The product name. Used as child key of the company's name.</param>
		/// <param name="regNode">Registy Node (HKEY_CURRENT_USER or HKEY_LOCAL_MACHINE) selection to use.</param>
		public RegistryIni(string companyName, string productName, RegistryNode regNode){
			this.companyName = companyName ;
			this.productName = productName ;
			this.regNode = regNode ;
			Init() ;
		}
		
		/// <summary>
		/// Initialize the object, creating the specified Registry Key to contact with.
		/// </summary>
		private void Init(){
			if(regNode == RegistryNode.CurrentUser){
				baseAppKey = Registry.CurrentUser.OpenSubKey("Software",true) ;
			}else{
				baseAppKey = Registry.LocalMachine.OpenSubKey("Software",true) ;
			}
			if(companyName != null){
				baseAppKey.CreateSubKey(companyName) ;
				baseAppKey = baseAppKey.OpenSubKey(companyName, true) ;
			}
			baseAppKey.CreateSubKey(productName) ;
			baseAppKey = baseAppKey.OpenSubKey(productName, true) ;
		}
		
		/// <summary>
		/// Access properties like Dictionary, negotiating with the property value name eg: registryIniObj["valueName"]<br>
		/// When getting value of the specified property, or empty string if property not found<br>
		/// When setting a value for a non-existed property, the property will be automatically added.<br>
		/// If the setting value is <em>null</em> the property will be deleted.
		/// </summary>
		public string this[string name]{
			get{
				return this[emptyStrArray, name] ;
			}
			set{
				this[emptyStrArray, name] = value ;
			}
		}
		
		/// <summary>
		/// Access properties like Dictionary, negotiating with the property sub key & value name eg: registryIniObj["subKeyName","valueName"]<br>
		/// When getting value of the specified property, or empty string if property not found<br>
		/// When setting a value for a non-existed subkey-value name pair, the subkey-value name pair will be automatically added.<br>
		/// If the setting value is <em>null</em> the property will be deleted.<br>
		/// <em>You can also specify a subkeys path, by separating the parent subkeys inside the subkey name string, e.g. "a\\b\\c" to specify the path app_basic_key\a\b\c</em>
		/// </summary>
		public string this[string subKey, string name]{
			get{
				return this[subKey.Split(keySeparator), name] ;
			}
			set{
				this[subKey.Split(keySeparator), name] = value  ;
			}
		}
		
		/// <summary>
		/// Access properties like Dictionary, negotiating with the property subkeys path & value name eg: registryIniObj[["subKeyA","subKeyB",..],"valueName"]<br>
		/// When getting value of the specified property, or empty string if property not found<br>
		/// When setting a value for a non-existed subkeys path-value name pair, the subkeys path-value name pair will be automatically added.<br>
		/// If the setting value is <em>null</em> the property will be deleted.<br>
		/// <em>Each right array element represent a child for the element at the left. e.g.: ["a","b","c"] parentSubKeys means the subKey under the app_basic_key\a\b\c\</em>
		/// </summary>
		public string this[string[] subKeys, string name]{
			get{
				RegistryKey key = baseAppKey ;
				foreach(string subKey in subKeys){
					key = key.OpenSubKey(subKey, true) ;
					if(key == null)
						return "" ;
				}
				
				object val = key.GetValue(name) ;
				if(val != null){
					return val.ToString() ;
				}else{
					return "" ;
				}
			}
			set{
				RegistryKey key = baseAppKey ;
				foreach(string subKey in subKeys){
					key.CreateSubKey(subKey) ;
					key = key.OpenSubKey(subKey, true) ;
				}
				
				if(value != null){
					key.SetValue(name, value) ;
				}else{
					key.DeleteValue(name) ;
				}
			}
		}
		
		/// <summary>
		/// Delete the subkey, its parent path specified in the array. Each right array element represent a child for the element at the left. 
		/// e.g.: ["a","b","c"] parentSubKeys means the subKey under the app_basic_key\a\b\c\ <strong>Use it with care! Can't be undone.</strong>
		/// </summary>
		/// <param name="parentSubKeys">The parent subkey path in array format</param>
		/// <param name="subKey">The child subkey name</param>
		public void DeleteSubKey(string[] parentSubKeys, string subKey){
			RegistryKey key = baseAppKey ;
			foreach(string parentSubKey in parentSubKeys){
				key = key.OpenSubKey(parentSubKey, true) ;
			}
			key.DeleteSubKey(subKey) ;
			key.Close() ;
		}
		
		/// <summary>
		/// Delete the subkey, under the specified parent path. Each parent key of the path, separated virtualy with a \ character. 
		/// e.g.: "a\\b\\c" parentSubKeysPath means the subKey under the app_basic_key\a\b\c\ <strong>Use it with care! Can't be undone.</strong>
		/// </summary>
		/// <param name="parentSubKeysPath">The parent subkey path</param>
		/// <param name="subKey">The child subkey name</param>
		public void DeleteSubKey(string parentSubKeysPath, string subKey){
			DeleteSubKey(parentSubKeysPath.Split(keySeparator), subKey) ;
		}
		
		/// <summary>
		/// Delete the specified subkey, which is located directly beneath the application basic key. <strong>Use it with care! Can't be undone.</strong>
		/// </summary>
		/// <param name="subKey">The subkey name</param>
		public void DeleteSubKey(string subKey){
			baseAppKey.DeleteSubKey(subKey) ;
		}
		
		/// <summary>
		/// Delete the specified subkey tree. <strong>Use it with care! Can't be undone.</strong>
		/// </summary>
		/// <param name="subKey">The parent subkey name</param>
		public void DeleteSubKeyTree(string subKey){
			baseAppKey.DeleteSubKeyTree(subKey) ;
		}
		
		/// <summary>
		/// Object deconstructor. Closing the basic application regisry key & flushes any unsaved changes.
		/// </summary>
		~RegistryIni(){
			baseAppKey.Close() ;
		}
	}
}