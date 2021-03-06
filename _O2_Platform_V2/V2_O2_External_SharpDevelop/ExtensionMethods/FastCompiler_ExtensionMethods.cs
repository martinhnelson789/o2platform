using System.Reflection;
using FluentSharp.O2.DotNetWrappers.ExtensionMethods;
using FluentSharp.O2.Kernel.ExtensionMethods;
using V2.O2.External.SharpDevelop.AST;
using FluentSharp.O2.DotNetWrappers.Windows;
using FluentSharp.O2.DotNetWrappers.DotNet;
using System;
using FluentSharp.O2.DotNetWrappers.H2Scripts;
using FluentSharp.O2.Kernel;

namespace V2.O2.External.SharpDevelop.ExtensionMethods
{
    public static class FastCompiler_ExtensionMethods
    {
        public static Assembly compile(this string pathToFileToCompile)
        {
            return pathToFileToCompile.compile(false);
        }

        public static Assembly compile(this string pathToFileToCompile, bool generateDebugSymbols)
        {
            PublicDI.CurrentScript = pathToFileToCompile;
            var csharpCompiler = new CSharp_FastCompiler();
            csharpCompiler.generateDebugSymbols= generateDebugSymbols;
            var compileProcess = new System.Threading.AutoResetEvent(false);
            csharpCompiler.compileSourceCode(pathToFileToCompile.contents());
            csharpCompiler.onCompileFail = () => compileProcess.Set();
            csharpCompiler.onCompileOK = () => compileProcess.Set();
            compileProcess.WaitOne();
            return csharpCompiler.assembly();
        }
        public static Assembly compile_CodeSnippet(this string codeSnipptet)
        {
            return codeSnipptet.compile_CodeSnippet(false);
        }

        public static Assembly compile_CodeSnippet(this string codeSnipptet, bool generateDebugSymbols)
        {   
            //Note we can't use the precompiled engines here since there is an issue of the resolution of this code dependencies

            var csharpCompiler = new CSharp_FastCompiler();
            csharpCompiler.generateDebugSymbols= generateDebugSymbols;
            var compileProcess = new System.Threading.AutoResetEvent(false);
            //csharpCompiler.compileSourceCode(pathToFileToCompile.contents());
            csharpCompiler.compileSnippet(codeSnipptet);
            csharpCompiler.onCompileFail = () => compileProcess.Set();
            csharpCompiler.onCompileOK = () => compileProcess.Set();
            compileProcess.WaitOne();
            var assembly = csharpCompiler.assembly();
            return assembly;
        }

        public static Assembly compile_H2Script(this string h2Script)
        {
            try
            {                
                var sourceCode = "";
                if (h2Script.extension(".h2"))
                {
                    PublicDI.CurrentScript = h2Script;
                    if (h2Script.fileExists().isFalse())
                        h2Script = h2Script.local();
                    sourceCode = H2.load(h2Script).SourceCode;
                }
                if (sourceCode.valid())
                    return sourceCode.compile_CodeSnippet();
            }
            catch (Exception ex)
            {
                ex.log("in string compile_H2Script");
            }
            return null;
        }

        public static Assembly assembly(this CSharp_FastCompiler csharpCompiler)
        {
            if (csharpCompiler != null)
                if (csharpCompiler.CompilerResults != null)
                    if (csharpCompiler.CompilerResults.Errors.HasErrors == false)
                    {
                        if (csharpCompiler.CompilerResults.CompiledAssembly != null)
                            return csharpCompiler.CompilerResults.CompiledAssembly;
                    }
                    else
                        "CompilationErrors:".line().add(csharpCompiler.CompilationErrors).error();

            return null;
        }

        public static Assembly compile(this string pathToFileToCompile, string targetAssembly)
        {
            var assembly = pathToFileToCompile.compile(true);
            Files.Copy(assembly.Location, targetAssembly);
            return assembly;
        }

        /*public static Assembly compile(this string pathToFileToCompile, bool compileToFileAndWithDebugSymbols)
        {
            string generateDebugSymbolsTag = @"//debugSymbols".line();
            if (pathToFileToCompile.fileContains(generateDebugSymbolsTag).isFalse())
                pathToFileToCompile.fileInsertAt(0, generateDebugSymbolsTag);
            return pathToFileToCompile.compile();

        } */       

        public static object executeFirstMethod(this string pathToFileToCompileAndExecute)
        {
            try
            {
                return pathToFileToCompileAndExecute.executeFirstMethod(new object[] { });
            }
            catch (Exception ex)
            {
                ex.log("in executeFirstMethod");
                return null;
            }
        }

        public static object executeFirstMethod(this string pathToFileToCompileAndExecute, object[] parameters)
        {
            PublicDI.CurrentScript = pathToFileToCompileAndExecute;
            if (pathToFileToCompileAndExecute.fileExists().isFalse())
            { 
                // if we were not provided a complete path, try to find it on the local o2 script folder                
                pathToFileToCompileAndExecute = pathToFileToCompileAndExecute.local();                
            }
            if (pathToFileToCompileAndExecute.extension(".h2"))
                return executeH2Script(pathToFileToCompileAndExecute);
            else
            {
                var assembly = pathToFileToCompileAndExecute.compile(true /* generatedDebug symbols */);
                return assembly.executeFirstMethod(parameters);
            }
        }

        public static object executeFirstMethod(this Assembly assembly)
        {
            return assembly.executeFirstMethod(false, false, new object[] {});
        }

        public static object executeFirstMethod(this Assembly assembly ,  object[] parameters)
        {
            return assembly.executeFirstMethod(false, false, parameters);
        }

        public static object executeFirstMethod(this Assembly assembly ,  bool executeInStaThread, bool executeInMtaThread, object[] parameters)
        {            
            if (assembly != null)
            {
                var methods = assembly.methods();
                foreach (var method in methods)
                    if (method.IsSpecialName == false && method.IsPublic)  // we need to do this since Properties get_ and set_ also look like methods
                    //if (methods.Count >0)        		
                    //{
                    {
                        if (executeInStaThread)
                            return O2Thread.staThread(() => method.executeMethod(parameters));
                        if (executeInMtaThread)
                            return O2Thread.mtaThread(() => method.executeMethod(parameters));

                        return method.executeMethod(parameters);
                    }
            }
            return null;
        }

        public static object executeMethod(this MethodInfo method, params object[] parameters)
        {
            try
            {
                if (method.parameters().size() == parameters.size())
                    return method.invoke(parameters);
                return method.invoke();
            }
            catch (Exception ex)
            {
                ex.log("in CSharp_FastCompiler.executeMethod");
                return null;
            }
        }
        public static object executeCodeSnippet(this string sourceCodeToExecute)
        {
            return sourceCodeToExecute.executeCodeSnippet();
        }
        public static object executeSourceCode(this string sourceCodeToExecute)
        {
            try
            {
                var assembly = sourceCodeToExecute.compile_CodeSnippet(true);
                return assembly.executeFirstMethod();
            }
            catch (Exception ex)
            {
                ex.log("in CSharp_FastCompiler.executeSourceCode");
                return null;
            }            
        }
        public static object executeH2Script(this string h2ScriptFile)
        {
            try
            {
                if (h2ScriptFile.extension(".h2").isFalse())
                    "[in executeH2Script]: file to execute must be a *.h2 file, it was:{0}".error(h2ScriptFile);
                else
                {
                    PublicDI.CurrentScript = h2ScriptFile;
                    var h2Script = H2.load(h2ScriptFile);
                    return h2Script.execute();                    
                }
            }
            catch (Exception ex)
            {
                ex.log("in CSharp_FastCompiler.executeH2Script");
            }
            return null;
        }

        public static object execute(this H2 h2Script)
        {
            try
            {                
                var assembly = h2Script.SourceCode.compile_CodeSnippet();
                return assembly.executeFirstMethod();                
            }
            catch (Exception ex)
            {
                ex.log("in CSharp_FastCompiler.executeH2Script");
            }
            return null;
        }
    }
}
