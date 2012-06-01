﻿using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.DotNetWrappers.DotNet; 
using O2.DotNetWrappers.Windows; 
using O2.XRules.Database.Utils;
using O2.External.SharpDevelop.ExtensionMethods;
using System.Runtime.InteropServices;
namespace O2.XRules.Database.APIs
{
	public class API_O2_Injector
	{
		public bool injectIntoProcess(Process process)
		{
			injectIntoProcess(process.Id, true);
			injectIntoProcess(process.Id, false);
			return true;
		}
		public bool injectIntoProcess(int processId, bool x64 = false)
		{
		 	var codeFile = "Injected_Dll.cs".local();
		 	var fixedCourceCode = codeFile.fileContents()	//ensure that we are pointing to the current locations of O2 folders
		 							 	  .replace(@"E:\O2_V4\O2.Platform.Projects\binaries",PublicDI.config.CurrentExecutableDirectory)
		 							 	  .replace(@"E:\O2_V4\O2.Platform.Scripts", PublicDI.config.LocalScriptsFolder);
		 	//fixedCourceCode.showInCodeViewer();
			var compiledAssembly = new CompileEngine().compileSourceCode(fixedCourceCode);
			if (compiledAssembly.notNull())
			{
				var className = "O2.Script.Test"; //"Snoop.SnoopUI";
				var methodName = "GoBabyGo";
				return injectIntoProcess(processId, x64, compiledAssembly.Location, className, methodName);
			}
			return false;
		}
		
		public bool injectIntoProcess(int processId, bool x64, string assemblyToInject, string className, string methodName)
		{		
			try
			{
				var suffix = (x64) ? "64-4.0" : "32-4.0";
				"Injecting into process {0} dll {1}".info(processId, assemblyToInject);
				var process = Processes.getProcess(processId);
				if (process.isNull())
				{
					"Could not find process with Id: {0}".error(processId);
					return false;
				}
				//var process = Processes.getProcess(12168);
				var hwnd = process.MainWindowHandle;
//				var hwnd = process.Handle;
				//var hwnd = new IntPtr(398680);
				if (hwnd == IntPtr.Zero)
				{
				 	"Could not get MainWindow handle".error();
				 	return false;
				}
				"Got main Window Handle: {0}".info(hwnd);	
				
				var snoopAssembly = @"Snoop\Snoop.exe".assembly(); 
				var directoryName = snoopAssembly.Location.directoryName();
				//return process.MainModule;
//				var suffix = snoopAssembly.type("Injector")
//										   .invokeStatic("Suffix", hwnd);
//				suffix = "64-4.0";
				var fileName = directoryName.pathCombine("ManagedInjectorLauncher" + suffix+ ".exe");							  
				
				
				var windowHandle= hwnd;							
	
				var arguments = string.Concat(new object[]
					{
						windowHandle, " \"", assemblyToInject,"\" \"", className, "\" \"", methodName,"\""
					});
				"Starting process {0} with arguments {1}".info(fileName, arguments);
				fileName.startProcess(arguments);
				return true;
			}
			catch(Exception ex)
			{
				ex.logStackTrace();
				return false;
			}		
		}
	}
	
	public static class API_O2_Injector_ExtensionMethods
	{
		public static Process process_MainWindow_BringToFront(this Process process)
		{
			if (process.MainWindowHandle != IntPtr.Zero)
				"WindowsBase.dll".assembly()
							 	.type("UnsafeNativeMethods")					 
							 	.invokeStatic("SetForegroundWindow",new HandleRef(null, process.MainWindowHandle)) ;
			else
				"[process_MainWindow_BringToFront] provided process has no main Window".error();
			return process;
		}
	}
}