﻿using System;
using System.Diagnostics;
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class testInstall
	{
		public static void test()  
		{
			new cURL_Install().start(); 
		}
	}
	 
	public class cURL_Install : Tool_API    
	{				
		public cURL_Install()
		{			
			config("cURL",
				   "http://curl.haxx.se/gknw.net/7.26.0/dist-w32/curl-7.26.0-rtmp-ssh2-ssl-sspi-zlib-idn-static-bin-w32.zip".uri(),
				   @"curl-7.26.0-rtmp-ssh2-ssl-sspi-zlib-idn-static-bin-w32\curl.exe");
			installFromZip_Web();	       		
		}	
		//
		
		public Process start()
		{			
			if (this.isInstalled())
				return this.Executable.startProcess("--help"); 
			return null;
		}		
	}
}