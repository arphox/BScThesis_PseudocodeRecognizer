﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LexicalAnalysisTests.Properties {
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
    internal class Inputs {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Inputs() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LexicalAnalysisTests.Properties.Inputs", typeof(Inputs).Assembly);
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
        ///egész x=2
        ///ha x&gt;=2 akkor
        ///	kiír &quot;x nem kisebb kettőnél...&quot;
        ///különben
        ///	kiír &quot;x kisebb, mint kettő!&quot;
        ///elágazás_vége
        ///egész[] y = létrehoz(egész)[10]
        ///ciklus egész i=0-tól 9-ig
        ///	y[i]=i
        ///	kiír y[i]
        ///ciklus_vége
        ///program_vége.
        /// </summary>
        internal static string ArrayForIf {
            get {
                return ResourceManager.GetString("ArrayForIf", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //komment
        ///program_kezd
        ///
        ///kiír     &quot;H//ello világ!&quot; //Ez egy egysoros komment
        /////kiír    &quot;Hello világ!&quot; //Ez egy egysoros komment
        ////*
        ///Elvileg működnie kellene. :P
        /////
        ///*/
        ///szöveg alma=&quot;almavagyok&quot;
        ///program_vége.
        /// </summary>
        internal static string Comments {
            get {
                return ResourceManager.GetString("Comments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///
        ///egész[] tömb = létrehoz(egész)[10]
        ///
        ///egész a = 2
        ///ciklus egész aa = 0-tól aa &lt; 9-ig
        ///	tört aaa = 2,4
        ///ciklus_vége
        ///
        ///
        ///egész b = 0
        ///ciklus egész bb = 0-tól bb &lt; 9-ig
        ///	szöveg bbb = &quot;alma&quot;
        ///	ha bb mod 2 == 0 akkor
        ///		szöveg bbbb = &quot;asd&quot;
        ///	elágazás_vége
        ///ciklus_vége
        ///
        ///egész c = 0
        ///ciklus_amíg c &lt; 2
        ///	egész ccc = 2
        ///	ha ccc == 3 akkor
        ///		ciklus egész cccc = 1-től cccc &lt; 10-ig
        ///			tört ccccc = 3,14
        ///			ciklus_amíg ccc &gt; 2
        ///				ccc = ccc - 2
        ///			ciklus_vége
        ///			egész ccccc2
        ///		ciklus_vége
        ///		lo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DeepBlocks {
            get {
                return ResourceManager.GetString("DeepBlocks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///logikai éhes = igaz
        ///ha éhes == igaz vagy hamis akkor
        ///	kiír &quot;menj enni!&quot;
        ///elágazás_vége
        ///szöveg konkatenált = &quot;valami&quot;.&quot;mégvalami&quot;.&quot; &quot;.&quot;még valami&quot;
        ///tört törtpélda=(+-6,0*+++10-(--0,3*+4,1)/--28,3-4)
        ///kiír törtpélda
        ///program_vége.
        /// </summary>
        internal static string Expressions {
            get {
                return ResourceManager.GetString("Expressions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///szöveg x=&quot;alma&quot;
        ///ciklus egész i=1-től i&lt;10-ig
        ///	kiír x
        ///ciklus_vége
        ///program_vége.
        /// </summary>
        internal static string For {
            get {
                return ResourceManager.GetString("For", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///
        ///egész[] tömb = létrehoz(egész)[10]
        ///egész a = törtből_egészbe(tömb[0] * 2,5) + logikaiból_egészbe(igaz)
        ///szöveg sz = szövegből_egészbe(a)
        ///logikai log = igaz
        ///tört tttt = logikaiból_törtbe(log)
        ///tttt = szövegből_törtbe(sz)
        ///log = egészből_logikaiba(123231)
        ///ciklus egész b = 0-tól b &lt; 9-ig
        ///	logikai xxxxxxxx = törtből_logikaiba(0,0)
        ///ciklus_vége
        ///szöveg sx = &quot;hamis&quot;
        ///log = szövegből_logikaiba(sx)
        ///program_vége.
        /// </summary>
        internal static string InternalFunctions {
            get {
                return ResourceManager.GetString("InternalFunctions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///	egész a
        ///	beolvas a
        ///	egész b
        ///	beolvas b
        ///	egész c
        ///	beolvas c
        ///	tört diszkrimináns=b*b-(4*a*c)
        ///	ha diszkrimináns&lt;0,0 akkor
        ///		kiír &quot;Nincs valós gyöke!&quot;
        ///	különben
        ///		kiír &quot;Van legalább egy valós gyöke!&quot;
        ///	elágazás_vége
        ///program_vége.
        /// </summary>
        internal static string Masodfoku {
            get {
                return ResourceManager.GetString("Masodfoku", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///egész x = 2
        ///program_kezd
        ///x = x + 1
        ///program_vége.
        /// </summary>
        internal static string MultipleStart {
            get {
                return ResourceManager.GetString("MultipleStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///egész x = 2
        ///x = x + 1.
        /// </summary>
        internal static string NoEnd {
            get {
                return ResourceManager.GetString("NoEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to egész x = 2
        ///x = x + 1
        ///program_vége.
        /// </summary>
        internal static string NoStart {
            get {
                return ResourceManager.GetString("NoStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to alma körte barack
        ///nincs is értelmes
        ///kód a fájlban!
        ///Jaj..
        /// </summary>
        internal static string NoStartEnd {
            get {
                return ResourceManager.GetString("NoStartEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to egész x = 2
        ///x = x + 1
        ///program_kezd
        ///egész x = 2
        ///x = x + 1
        ///egész a = 2
        ///ciklus egész b = 0-tól b &lt; 9-ig
        ///	egész c = törtből_egészbe(2,4)
        ///ciklus_vége
        ///program_vége
        ///egész x = 2
        ///x = x + 1
        ///ciklus_vége
        ///program_vége
        ///program_kezd
        ///program_vége.
        /// </summary>
        internal static string NotOnlyCode {
            get {
                return ResourceManager.GetString("NotOnlyCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///x = x + 1
        ///program_vége.
        /// </summary>
        internal static string NoType {
            get {
                return ResourceManager.GetString("NoType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///egész a
        ///egész b
        ///logikai a
        ///egész[] tömb = létrehoz(egész)[10]
        ///szöveg error
        ///logikai lenniVAGYnemLENNI
        ///tört burgonya = 2,3
        ///program_vége.
        /// </summary>
        internal static string Redeclaration {
            get {
                return ResourceManager.GetString("Redeclaration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///
        ///egész[] tömb = létrehoz(egész)[10]
        ///
        ///ciklus egész i=0-tól i&lt;9-ig
        ///	tömb[i] = i*10
        ///	kiír tömb[i]
        ///ciklus_vége
        ///
        ///
        ///
        ///egész db=0
        ///ciklus egész i=0-tól i&lt;9-ig
        ///	ha tömb[i]mod 2==0 akkor
        ///		db=db+1
        ///	elágazás_vége
        ///ciklus_vége
        ///kiír &quot;Ennyi darab páros \&quot;szám van: &quot;.db
        ///
        ///egész maxi=0
        ///ciklus egész i=0-tól i&lt;9-ig
        ///	ha tömb[i]&gt;tömb[maxi]
        ///		maxi=i
        ///	elágazás_vége
        ///ciklus_vége
        ///kiír &quot;A maximális elem: tömb[&quot;.maxi.&quot;]=&quot;.tömb[maxi]
        ///egész xxx = szövegből_egészbe(&quot;10&quot;)
        ///program_vége.
        /// </summary>
        internal static string SimpleTheorems {
            get {
                return ResourceManager.GetString("SimpleTheorems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to program_kezd
        ///egész x = 2;
        ///egész y = 3;
        ///y = x + y;
        ///x = x + 1;
        ///program_vége.
        /// </summary>
        internal static string UnknownSymbol {
            get {
                return ResourceManager.GetString("UnknownSymbol", resourceCulture);
            }
        }
    }
}
