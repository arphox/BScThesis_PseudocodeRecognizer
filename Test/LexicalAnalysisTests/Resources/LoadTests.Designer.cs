﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LexicalAnalysisTests.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class LoadTests {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal LoadTests() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LexicalAnalysisTests.Resources.LoadTests", typeof(LoadTests).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///
        ///// Hogy ne legyen probléma az újradeklarálással, felvettem itt mindet:
        ///egész N 
        ///egész[] tömb
        ///egész összeg
        ///egész darab
        ///egész maxi
        ///
        ///// Létrehozás
        ///N = 10
        ///tömb = létrehoz(egész)[N]
        ///
        ///// Feltöltés
        ///ciklus egész i = 0-tól i &lt; N-ig
        ///	tömb[i] = i * 10
        ///ciklus_vége
        ///
        /////  Összegzés
        ///összeg = 0
        ///ciklus egész i = 0-tól i&lt; N-ig
        ///	összeg = összeg + tömb[i]
        ///ciklus_vége
        ///kiír &quot;A tömb elemeinek összege &quot;.összeg
        ///
        ///// Megszámlálás
        ///darab = 0
        ///ciklus egész i = 0-tól i&lt; N-ig
        ///	ha tömb[i] mod 2 == 0  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string loadtest10k {
            get {
                return ResourceManager.GetString("loadtest10k", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///
        ///// Hogy ne legyen probléma az újradeklarálással, felvettem itt mindet:
        ///egész N 
        ///egész[] tömb
        ///egész összeg
        ///egész darab
        ///egész maxi
        ///
        ///// Létrehozás
        ///N = 10
        ///tömb = létrehoz(egész)[N]
        ///
        ///// Feltöltés
        ///ciklus egész i = 0-tól i &lt; N-ig
        ///	tömb[i] = i * 10
        ///ciklus_vége
        ///
        /////  Összegzés
        ///összeg = 0
        ///ciklus egész i = 0-tól i&lt; N-ig
        ///	összeg = összeg + tömb[i]
        ///ciklus_vége
        ///kiír &quot;A tömb elemeinek összege &quot;.összeg
        ///
        ///// Megszámlálás
        ///darab = 0
        ///ciklus egész i = 0-tól i&lt; N-ig
        ///	ha tömb[i] mod 2 == 0  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string loadtest1k {
            get {
                return ResourceManager.GetString("loadtest1k", resourceCulture);
            }
        }
    }
}