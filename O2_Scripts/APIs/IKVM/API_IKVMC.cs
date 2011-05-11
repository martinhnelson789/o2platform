// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Zip;
using O2.XRules.Database.Utils;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;

//O2File:API_IKVMC_JavaMetadata.cs
//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
//O2Ref:IKVM.Runtime.dll
//O2Ref:IKVM.Runtime.JNI.dll
//O2Ref:IKVM.OpenJDK.Util.dll
//O2Ref:IKVM.OpenJDK.Core.dll
//O2Ref:IKVM.Reflection.dll
//O2Ref:ikvmc.exe
//O2Ref:ICSharpCode.SharpZipLib.dll

namespace O2.XRules.Database.APIs.IKVM
{
    public class API_IKVMC	
    {
    	public Assembly IkvmcAssembly   { get; set;}
    	public Type StaticCompiler  { get; set;}
    	public object IkvmRuntime 	  { get; set;}
    	public object IkvmRuntimeJni  { get; set;}
    	public object IkvmcCompiler   { get; set;}
    	public object CompilerOptions { get; set;}
    	
    	public API_IKVMC()
    	{
    		setup();
    	}
    	
    	public void setup()
    	{    		
    		IkvmcAssembly = "ikvmc.exe".assembly();
    		StaticCompiler = IkvmcAssembly.type("StaticCompiler");
			IkvmRuntime = StaticCompiler.invokeStatic("LoadFile",Environment.CurrentDirectory.pathCombine("IKVM.Runtime.dll")); 
			PublicDI.reflection.setField((FieldInfo)StaticCompiler.field("runtimeAssembly"),IkvmRuntime);  				
			
			IkvmRuntimeJni = StaticCompiler.invokeStatic("LoadFile",Environment.CurrentDirectory.pathCombine("IKVM.Runtime.JNI.dll")); 
			PublicDI.reflection.setField((FieldInfo)StaticCompiler.field("runtimeJniAssembly"),IkvmRuntimeJni);  
			
			IkvmcCompiler =  IkvmcAssembly.type("IkvmcCompiler").ctor();
			
			CompilerOptions = IkvmcAssembly.type("CompilerOptions").ctor();
			PublicDI.reflection.setField((FieldInfo)StaticCompiler.field("toplevel"),CompilerOptions);  
    	}
	}
	
	public static class API_IKVMC_ExtensionMethods_CreateJavaMetadata
	{
		public static Dictionary<string, byte[]> getRawClassesData_FromFile_ClassesOrJar(this API_IKVMC ikvmc, string classOrJar)
		{
			var targets = typeof(List<>).MakeGenericType(new System.Type[] { ikvmc.CompilerOptions.type()}).ctor();
			var args = classOrJar.wrapOnList().GetEnumerator();
			ikvmc.IkvmcCompiler.invoke("ParseCommandLine", args, targets, ikvmc.CompilerOptions);  				
			var compilerOptions =  (targets as IEnumerable).first();
			var classes =   (Dictionary<string, byte[]>) compilerOptions.field("classes");  
			return classes;
		}				
		
		public static object createClassFile(this API_IKVMC ikvmc, byte[] bytes)
		{
			// 0 = ClassFileParseOptions.None, 
			// 1 = ClassFileParseOptions.LocalVariableTable , 
			// 2 = ClassFileParseOptions.LineNumberTable
			return ikvmc.createClassFile(bytes,0);			
		}
		
		public static object createClassFile(this API_IKVMC ikvmc, byte[] bytes, int classFileParseOptions)
		{
			var classFileType = ikvmc.IkvmcAssembly.type("ClassFile");
			var ctor = classFileType.ctors().first();					
		 
			var classFile = ctor.Invoke(new object[] {bytes,0, bytes.Length, null, classFileParseOptions });   
			if (classFile.notNull())
				return classFile;
			"[API_IKVMC] in createClassObject failed to create class".info();
			return null;
		}
		 
		public static API_IKVMC_Java_Metadata create_JavaMetadata(this API_IKVMC ikvmc, string fileToProcess)
		{
			var o2Timer = new O2Timer("Created JavaData for {0}".format(fileToProcess)).start();
			var javaMetaData = new API_IKVMC_Java_Metadata();
			javaMetaData.FileProcessed = fileToProcess;
			var classes = ikvmc.getRawClassesData_FromFile_ClassesOrJar(fileToProcess);
			foreach(var item in classes)
			{
				var name = item.Key;
				var bytes = item.Value;
				var classFile = ikvmc.createClassFile(bytes,1);				// value of 1 gets the local variables
				var javaClass = new Java_Class {						
													Signature = name,
													Name = name.contains(".") ? name.split(".").last() : name,
													SourceFile = classFile.prop("SourceFileAttribute").str(),
													IsAbstract = classFile.prop("IsAbstract").str(),
													IsInterface = classFile.prop("IsInterface").str(),
													IsInternal = classFile.prop("IsInternal").str(),
													IsPublic = classFile.prop("IsPublic").str(),
													SuperClass = classFile.prop("SuperClass").str()
											 };
				
				javaClass.ConstantsPool = classFile.getConstantPoolEntries();
				javaClass.map_Annotations(classFile)
						 .map_Interfaces(classFile)
						 .map_Fields(classFile)
						 .map_Methods(classFile);
						 				
				javaClass.map_LineNumbers(ikvmc.createClassFile(bytes,2)); // for this we need to call createClassFile with the value of 2 (to get the source code references)
				javaMetaData.Classes.Add(javaClass);												
				//break; // 
			}
			o2Timer.stop();
			return javaMetaData;
		}
		
		public static Java_Class map_Annotations(this Java_Class javaClass, object classFile)
		{
			var annotations = (Object[])classFile.prop("Annotations");
			if (annotations.notNull())			
				javaClass.Annotations = annotations;			
			return javaClass;
		}
		
		public static Java_Method map_Annotations(this Java_Method javaMethod, object method)
		{
			var annotations = (Object[])method.prop("Annotations");
			if (annotations.notNull())			
				javaMethod.Annotations = annotations;			
				
			var parameterAnnotations = (Object[])method.prop("ParameterAnnotations");
			if (parameterAnnotations.notNull())			
				javaMethod.ParameterAnnotations = parameterAnnotations;								
				
			return javaMethod;
		}
		
		public static Java_Class map_Interfaces(this Java_Class javaClass, object classFile)
		{
			var interfaces = (IEnumerable)classFile.prop("Interfaces");						
			foreach(var _interface in interfaces)
				javaClass.Interfaces.Add(_interface.prop("Name").str());				
			return javaClass;
		}
		
		public static Java_Class map_Fields(this Java_Class javaClass, object classFile)
		{
			//var interfaces = (IEnumerable)classFile.prop("Interfaces");						
			//foreach(var _interface in interfaces)
			//	javaClass.Interfaces.Add(_interface.prop("Name").str());				
			var fields = (IEnumerable)classFile.prop("Fields");
			foreach(var field in fields)			
				javaClass.Fields.Add(new Java_Field
										{
											Name = field.prop("Name").str(),
											Signature = field.prop("Signature").str(),
											ConstantValue= field.prop("ConstantValue").notNull() 
																? field.prop("ConstantValue").str()
																: null
										});				
			
			return javaClass;
		}
		
		public static Java_Class map_Methods(this Java_Class javaClass, object classFile)
		{
			var mappedConstantsPools = javaClass.ConstantsPool.getDictionaryWithValues();
			var methods = (IEnumerable)classFile.prop("Methods");
			foreach(var method in methods)
			{		
				var javaMethod = new Java_Method {
													Name = method.prop("Name").str(),
													ParametersAndReturnValue = method.prop("Signature").str(),													
													ClassName = javaClass.Signature,
													IsAbstract = method.prop("IsAbstract").str(), 
													IsClassInitializer = method.prop("IsClassInitializer").str(), 
													IsNative = method.prop("IsNative").str(), 
													IsPublic = method.prop("IsPublic").str(), 
													IsStatic = method.prop("IsStatic").str()
												 };
				javaMethod.Signature = "{0}{1}".format(javaMethod.Name,javaMethod.ParametersAndReturnValue);												 								
				
				javaMethod.map_Annotations(method)
						  .map_Variables(method);
				
				
				var instructions = (IEnumerable) method.prop("Instructions");
				if (instructions.notNull())
				{	
					foreach(var instruction in instructions)
					{
						var javaInstruction = new Java_Instruction();
						javaInstruction.Pc = instruction.prop("PC").str().toInt(); 
						javaInstruction.OpCode = instruction.prop("NormalizedOpCode").str(); 
						javaInstruction.TargetIndex =  instruction.prop("TargetIndex").str().toInt();
						javaMethod.Instructions.Add(javaInstruction);						
					}
				}
				
				javaClass.Methods.Add(javaMethod);
			}
			return javaClass;
		}
		
		public static Java_Method map_Variables(this Java_Method javaMethod, object method)
		{			
			var localVariablesTable = (IEnumerable)method.prop("LocalVariableTableAttribute");
			if (localVariablesTable.notNull())			
				foreach(var localVariable in localVariablesTable)
					javaMethod.Variables.Add(new Java_Variable  {
																	Descriptor = localVariable.field("descriptor").str(), 
																	Index = localVariable.field("index").str().toInt(), 
																	Length = localVariable.field("length").str().toInt(), 
																	Name = localVariable.field("name").str(), 
																	Start_Pc = localVariable.field("start_pc").str().toInt()
																});							
			return javaMethod;
		}
		
		//we need to do this because the IKVMC doesn't have a option to get both fields and source code references at the same time
		public static Java_Class map_LineNumbers(this Java_Class javaClass, object classFileWithLineNumbers)
		{
			var methodsWithLineNumbers = ((IEnumerable)classFileWithLineNumbers.prop("Methods"));
			var currentMethodId = 0;
			foreach(var methodWithLineNumbers in methodsWithLineNumbers)
			{
				var javaMethod = javaClass.Methods[currentMethodId];
				var lineNumberTableAttributes = (IEnumerable)methodWithLineNumbers.prop("LineNumberTableAttribute");				
				if (lineNumberTableAttributes.notNull())
				{
					foreach(var lineNumberTableAttribute in lineNumberTableAttributes)
						javaMethod.LineNumbers.Add(new LineNumber {
																	Line_Number = lineNumberTableAttribute.field("line_number").str().toInt(),
																	Start_Pc = lineNumberTableAttribute.field("start_pc").str().toInt()																		
																  });					
																  
					var  mapppedLineNumbers = new Dictionary<int,int>();
					foreach(var lineNumber in javaMethod.LineNumbers)
						mapppedLineNumbers.Add(lineNumber.Start_Pc, lineNumber.Line_Number);
					var lastLineNumber = 0;	
					foreach(var instruction in javaMethod.Instructions)
						if (mapppedLineNumbers.hasKey(instruction.Pc))
						{
							instruction.Line_Number = mapppedLineNumbers[instruction.Pc];
							lastLineNumber = instruction.Line_Number;
						}
						else
							instruction.Line_Number = lastLineNumber;
				}
				currentMethodId++;																  
			}						
			return javaClass;
		}
		
		public static List<ConstantPool> add_Entry(this List<ConstantPool> constantsPool,int id, string type, string value)
		{
			return constantsPool.add_Entry(id, type, value, false);
		}
		
		public static List<ConstantPool> add_Entry(this List<ConstantPool> constantsPool,int id, string type, string value, bool valueEncoded)
		{
			var constantPool = new ConstantPool(id, type, value,valueEncoded);
			constantsPool.Add(constantPool);
			return constantsPool;
		}
		public static List<ConstantPool> getConstantPoolEntries(this object classFile)
		{
			var constantsPool = new List<ConstantPool>();
			
			var constantPoolRaw = new Dictionary<int,object>();
			var constantPool =  (IEnumerable)classFile.field("constantpool");  
			var index = 0;
			foreach(var constant in constantPool)
				constantPoolRaw.add(index++, constant); 
				
			//var constantPoolValues = new Dictionary<int,string>();	 
		//	var stillToMap = new List<object>();
			for(int i=0; i < constantPoolRaw.size() ; i++)
			{
				var currentItem = constantPoolRaw[i];
				var currentItemType = currentItem.str();
				switch(currentItemType)
				{
					case "IKVM.Internal.ClassFile+ConstantPoolItemClass":											
						constantsPool.add_Entry(i, currentItemType.remove("IKVM.Internal.ClassFile+ConstantPoolItem"), currentItem.prop("Name").str());						
						break;
					case "IKVM.Internal.ClassFile+ConstantPoolItemMethodref":						
					case "IKVM.Internal.ClassFile+ConstantPoolItemFieldref":									
						constantsPool.add_Entry(i,currentItemType.remove("IKVM.Internal.ClassFile+ConstantPoolItem"), 
												"{0}.{1} : {2}".format(currentItem.prop("Class"),
												   	 				   currentItem.prop("Name"),
												   	 				   currentItem.prop("Signature")));									
						break;
					case "IKVM.Internal.ClassFile+ConstantPoolItemNameAndType":	 // skip this one since don;t know what they point to
						//constantPoolValues.Add(i,"IKVM.Internal.ClassFile+ConstantPoolItemNameAndType");	
						break; 
					case "IKVM.Internal.ClassFile+ConstantPoolItemString":
					case "IKVM.Internal.ClassFile+ConstantPoolItemInteger":
					case "IKVM.Internal.ClassFile+ConstantPoolItemFloat":
					case "IKVM.Internal.ClassFile+ConstantPoolItemDouble": 
					case "IKVM.Internal.ClassFile+ConstantPoolItemLong":
						var value = currentItem.prop("Value").str();
						value = value.base64Encode();//HACK to deal with BUG in .NET Serialization and Deserialization (to reseatch further)
						constantsPool.add_Entry(i,currentItemType.remove("IKVM.Internal.ClassFile+ConstantPoolItem"),value, true);
						break;
					case "IKVM.Internal.ClassFile+ConstantPoolItemInterfaceMethodref": // doesn't have any value to use
						break;
					case "[null value]":
						//constantsPool.add_Entry(i,"[null value]", null);						
						break; 
					default:
						"Unsupported constantPoll type: {0}".error(currentItem.str());
						break;
				}		
			}
			return constantsPool;	
		}		
		
		public static Dictionary<int,string> constantsPool_Values(this Java_Class _class)
		{
			return _class.ConstantsPool.getDictionaryWithValues();
		}
		
		public static Dictionary<int,ConstantPool> constantsPool_byIndex(this Java_Class _class)
		{
			return _class.ConstantsPool.getDictionary_byIndex();
		}
		
		public static Dictionary<string,List<ConstantPool>> constantsPool_byType(this Java_Class _class)
		{
			return _class.ConstantsPool.getDictionary_byType();
		}
		
		public static Dictionary<string,List<ConstantPool>> constantsPool_byType(this Java_Class _class, Java_Method method)
		{
			return _class.ConstantsPool.getDictionary_byType(method);
		}
		
		public static Dictionary<int,string> getDictionaryWithValues(this  List<ConstantPool> constantsPool)
		{
			var dictionary = new Dictionary<int,string>();
			foreach(var item in constantsPool)
			{
				if (item.ValueEncoded)
					dictionary.Add(item.Id, "\"{0}\"".format(item.Value.base64Decode()));	
				else
					dictionary.Add(item.Id, item.Value);					
			}
			return dictionary;
		}
		
		public static Dictionary<int,ConstantPool> getDictionary_byIndex(this  List<ConstantPool> constantsPool)
		{
			var dictionary = new Dictionary<int,ConstantPool>();
			foreach(var item in constantsPool)								
				dictionary.Add(item.Id, item);			
			return dictionary;
		}
		
		public static Dictionary<string,List<ConstantPool>> getDictionary_byType(this  List<ConstantPool> constantsPool)
		{
			var dictionary = new Dictionary<string,List<ConstantPool>>();
			foreach(var item in constantsPool)								
				dictionary.add(item.Type, item);			
			return dictionary;
		}
		
		public static Dictionary<string,List<ConstantPool>> getDictionary_byType(this  List<ConstantPool> constantsPool, Java_Method method)		
		{
			var usedInMethod = method.uniqueTargetIndexes();
			//show.info(usedInMethod);
			var mappedByIndex = constantsPool.getDictionary_byIndex();
			//show.info(mappedByIndex); 
			var dictionary = new Dictionary<string,List<ConstantPool>>();
			foreach(var index in usedInMethod)
			{
				if (mappedByIndex.hasKey(index))
				{
					var constantPool = mappedByIndex[index];
					dictionary.add(constantPool.Type, constantPool);
				}
			}
			return dictionary;
		}
		

	}
	
	public static class API_IKVMC_ExtensionMethods_CreateDotNetAssemblies
	{
		public static string createAssembly_FromFile_ClasssesOrJar(this API_IKVMC ikvmc, string classOrJar)
		{				
			var console = "IKVM Stored Console out and error".capture_Console();
			var targetFile = "_IKVM_Dlls".tempDir(false).pathCombine("{0}.dll".format(classOrJar.fileName()));
			
			var args = new List<string> {	
											classOrJar,
											"-out:{0}".format(targetFile)
										} .GetEnumerator();
			//return args.toList();								
			var targets = typeof(List<>).MakeGenericType(new System.Type[] { ikvmc.CompilerOptions.type()}).ctor();

			ikvmc.IkvmcCompiler.invoke("ParseCommandLine", args, targets, ikvmc.CompilerOptions); 

			//ikvmcCompiler.details();
			var compilerClassLoader = ikvmc.IkvmcAssembly.type("CompilerClassLoader");
			var compile = compilerClassLoader.method("Compile"); 
			
			var createCompiler = compilerClassLoader.method("CreateCompiler"); 
			
			PublicDI.reflection.invokeMethod_Static(compile, new object[] {null,targets});  
			//ikvmcCompiler.details(); 
			return console.readToEnd();
		}	
	}
	
	public static class API_IKVMC_ExtensionMethods_Gui_Helpers
	{
		public static TreeNode add_Instructions(this TreeNode treeNode, Java_Method method, Java_Class methodClass)
		{
			var values = methodClass.ConstantsPool.getDictionaryWithValues();
			foreach(var instruction in method.Instructions)
			{
				var nodeText = "[line:{0}] \t {1}".format(instruction.Line_Number,instruction.OpCode);
				if (instruction.TargetIndex > 0 && values.hasKey(instruction.TargetIndex))								
					nodeText = "{0} {1}".format(nodeText , values[instruction.TargetIndex]);
				treeNode.add_Node(nodeText, instruction)
						.color(Color.DarkGreen);
			
			}
			return treeNode;
		}
		
		public static TreeNode add_Variables(this TreeNode treeNode, Java_Method method)
		{
			if (method.Variables.size()>0)
			{
				var variablesNode = treeNode.add_Node("_Variables");
				foreach(var variable in method.Variables)
					variablesNode.add_Node("{0}  :   {1}".format(variable.Name , variable.Descriptor),variable);
			}
			return treeNode;
		}
		
		public static TreeNode add_Annotations(this TreeNode treeNode, bool addAnnotationsNode, object[] annotations)
		{
			if (annotations.notNull())
			{					
				if(addAnnotationsNode) 
					treeNode.add_Node("_Annotations", annotations, true);
				else									
					foreach(var annotation in annotations)
						treeNode.add_Node(annotation.str(),annotation, annotation is object[]);
			}
			return treeNode;
		}
		
		public static TreeNode add_ConstantsPool(this TreeNode treeNode, Java_Class _class)
		{
			var constantsPoolNode = treeNode.add_Node("_ConstantsPool");
			foreach(var item in _class.constantsPool_byType())
				constantsPoolNode.add_Node(item.Key, item.Value, true);
			return treeNode;
		}
		
		public static TreeNode add_ConstantsPool(this TreeNode treeNode, Java_Method method, Java_Class methodClass)
		{
			var constantsPoolNode = treeNode.add_Node("_ConstantsPool");
			foreach(var item in methodClass.constantsPool_byType(method))
				constantsPoolNode.add_Node(item.Key, item.Value, true);
			return treeNode;
		}
		
		
		public static ascx_SourceCodeViewer showInCodeViewer(this ascx_SourceCodeViewer codeViewer ,Java_Class _class, Java_Method method)
		{
			codeViewer.editor().showInCodeEditor(_class, method);
			return codeViewer;
		}
		
		public static ascx_SourceCodeEditor showInCodeEditor(this ascx_SourceCodeEditor codeEditor ,Java_Class _class, Java_Method method)
		{										
			//var _class = classes_bySignature[classSignature];
			var file = _class.Signature.remove(_class.Name);
			file = "{0}{1}".format(file.replace(".","\\"), _class.SourceFile); 
			codeEditor.open(file);
			var lineNumber = 0; 
			foreach(var item in method.LineNumbers)
				if (item.Line_Number > 1)
					if (lineNumber == 0 || item.Line_Number < lineNumber)
						lineNumber = item.Line_Number;								
			codeEditor.gotoLine(lineNumber);			
			return codeEditor;
   		}
			
		public static T viewJavaMappings<T>(this T control, API_IKVMC_Java_Metadata javaMappings)
			where T : Control
		{	
			control.clear();
			Dictionary<string, Java_Class> classes_bySignature = null;
			var showFullClassNames = false;
			var openSourceCodeReference = false;
			var treeView = control.add_TreeView_with_PropertyGrid();
			var codeEditor = control.insert_Right().add_SourceCodeEditor();
			var configPanel = control.insert_Below(40,"Config");
			
			configPanel.add_CheckBox("Show Full Class names", 0,0, 
							(value)=>{
										showFullClassNames = value;
										treeView.collapse();
									 }).autoSize()
					   .append_Control<CheckBox>().set_Text("Open SourceCode Reference")
					   							  .autoSize()					   							  
					   							  .onChecked((value)=> openSourceCodeReference=value)
					   							  .check();
			//BeforeExpand Events
			treeView.beforeExpand<API_IKVMC_Java_Metadata>(
				(treeNode, javaMetadata)=>{
												treeNode.add_Nodes(javaMetadata.Classes.OrderBy((item)=>item.Name), 	
																   (_class)=> (showFullClassNames)
																   					? _class.Signature
																   					: _class.Name ,
																   (_class)=> true,
																   (_class)=> Color.DarkOrange);
										  });
										  
			treeView.beforeExpand<Java_Class>(   
				(treeNode, _class)=>{
										treeNode.add_ConstantsPool(_class)
												.add_Annotations(true, _class.Annotations);
										treeNode.add_Nodes(_class.Methods.OrderBy((item)=>item.Name),
														   (method)=> method.str(),
														   (method)=> true,
													 	   (method)=> Color.DarkBlue);
									});
										    
			treeView.beforeExpand<Java_Method>( 
				(treeNode, method)=>{  
										treeNode.add_ConstantsPool(method,classes_bySignature[method.ClassName])
												.add_Annotations(true, method.Annotations)
											    .add_Variables(method);

										treeNode.add_Node("_Instructions")  											    
												.add_Instructions(method,classes_bySignature[method.ClassName]); 
																		
									}); 
									
			treeView.beforeExpand<object[]>(     
				(treeNode, annotations)=>{
											treeNode.add_Annotations(false, annotations);
										});		
			
			treeView.beforeExpand<List<ConstantPool>>(
				(treeNode, constantsPool)=>{
											treeNode.add_Nodes(constantsPool.OrderBy((item)=>item.str()),
															   (constant)=> constant.str(),
														       (constant)=> false,
													 	       (constant)=> Color.Sienna);
										});												
			//AfterSelect Events
						  
			treeView.afterSelect<Java_Class>(  
				(_class) => { 
								if (openSourceCodeReference)
								{
									codeEditor.showInCodeEditor(classes_bySignature[_class.Signature], _class.Methods.first());	
									treeView.focus();  
								}
							});
			
			treeView.afterSelect<Java_Method>(
				(method) => {
								if (openSourceCodeReference)
								{
									codeEditor.showInCodeEditor(classes_bySignature[method.ClassName], method); 
									treeView.focus();  
								}
							});
							
			treeView.afterSelect<Java_Instruction>(
				(instruction) => {	
									if (openSourceCodeReference)
									{
										codeEditor.gotoLine(instruction.Line_Number); 
										treeView.focus();  
									}
							});				
						
			//Other events
			Action<API_IKVMC_Java_Metadata> loadJavaMappings =
				(_javaMappings)=>{		
									if (_javaMappings.notNull())
									{
										treeView.clear();
										classes_bySignature =  _javaMappings.getClasses_IndexedBySignature();    			
										treeView.add_Node(_javaMappings.str(), _javaMappings, true); 				
										treeView.focus();
									}
									else
										treeView.add_Node("Drop Jar/Zip or class file to view its contents");
								 };
			treeView.onDrop((
				file)=>{
							treeView.azure();
							O2Thread.mtaThread(
								()=>{
										loadJavaMappings(new API_IKVMC().create_JavaMetadata(file));
										treeView.white();
									});
						});
						
			loadJavaMappings(javaMappings);			
		
			return control;
		}
	}	
}