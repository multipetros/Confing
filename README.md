# Confing Library
Copyright (c) 2015, [Petros Kyladitis](http://www.multipetros.gr/) 

## Description
This is a .NET library with classes for easily confinguration properties manipulation
There are three (3) classes. The two of them are contacting with flat text files to
store the properties and the other one with the MS Windows Registry.

The **SimpleIni** class create objects to manipulate properites inside text files, in a
dead-simple INI file format. Where each property stored in one line and the key-value
pair is separated with a separator characted. The '=' by default, but programer can
choose another one at the object initialization. Except these well known INI format
characteristicts, nothing else are supported. No sections or comments are allowed.
Because of these, the SimpleIni object is very light-weight and fast.  
  
The **Ini** class create objects to manipulate properites inside text files, with
full support of the INI file format. Sections, comments and in-line comments are
allowed, with also custom key-value separator and comments character.
  
The **RegIni** class create objects to manipulate properties in the MS Windows
Registry, by creating application subkeys inside the HKEY_CURRENT_USER\Software\ or the
HKEY_LOCAL_MACHINE\SOFTWARE\ node. Except the values at the top of the application
base key, unlimited depth subkey creation is supported.  

All classes are inherit from **ConfigBase**, static methods to add & remove quotes, and 
to encode & decode properties to [Base64](http://en.wikipedia.org/wiki/Base64). So, with 
these static methods, let you store properites values with spaces at the begin and/or the 
end and also, except the plain text, binary data can also be stored as value for the 
properties.


## License
This program is free software distributed under the FreeBSD,
for license details see at _'license.txt'_ file, distributed with
this program, or see at <http://www.multipetros.gr/freebsd-license/>.