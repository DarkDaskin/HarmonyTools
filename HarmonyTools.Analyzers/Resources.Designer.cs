﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HarmonyTools.Analyzers {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HarmonyTools.Analyzers.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Argument types and argument variations have differing number of items..
        /// </summary>
        internal static string ArgumentTypesAndVariationsMustMatchMessageFormat {
            get {
                return ResourceManager.GetString("ArgumentTypesAndVariationsMustMatchMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Argument types and argument variations mismatch.
        /// </summary>
        internal static string ArgumentTypesAndVariationsMustMatchTitle {
            get {
                return ResourceManager.GetString("ArgumentTypesAndVariationsMustMatchTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid attribute argument..
        /// </summary>
        internal static string AttributeArgumentsMustBeValidMessageFormat {
            get {
                return ResourceManager.GetString("AttributeArgumentsMustBeValidMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid attribute argument.
        /// </summary>
        internal static string AttributeArgumentsMustBeValidTitle {
            get {
                return ResourceManager.GetString("AttributeArgumentsMustBeValidTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple auxiliary patch methods of the same kind are defined. Only the first one will be executed..
        /// </summary>
        internal static string DontDefineMultipleAuxiliaryPatchMethodsMessageFormat {
            get {
                return ResourceManager.GetString("DontDefineMultipleAuxiliaryPatchMethodsMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple auxiliary patch methods of the same kind are defined.
        /// </summary>
        internal static string DontDefineMultipleAuxiliaryPatchMethodsTitle {
            get {
                return ResourceManager.GetString("DontDefineMultipleAuxiliaryPatchMethodsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bulk patching can&apos;t be combined with individual target method annotations..
        /// </summary>
        internal static string DontUseIndividualAnnotationsWithBulkPatchingMessageFormat {
            get {
                return ResourceManager.GetString("DontUseIndividualAnnotationsWithBulkPatchingMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bulk patching can&apos;t be combined with individual target method annotations.
        /// </summary>
        internal static string DontUseIndividualAnnotationsWithBulkPatchingTitle {
            get {
                return ResourceManager.GetString("DontUseIndividualAnnotationsWithBulkPatchingTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only single method of bulk patching is allowed..
        /// </summary>
        internal static string DontUseMultipleBulkPatchingMethodsMessageFormat {
            get {
                return ResourceManager.GetString("DontUseMultipleBulkPatchingMethodsMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only single method of bulk patching is allowed.
        /// </summary>
        internal static string DontUseMultipleBulkPatchingMethodsTitle {
            get {
                return ResourceManager.GetString("DontUseMultipleBulkPatchingMethodsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing HarmonyPatch attribute on type. Specified patches will not be discovered..
        /// </summary>
        internal static string HarmonyPatchAttributeMustBeOnTypeMessageFormat {
            get {
                return ResourceManager.GetString("HarmonyPatchAttributeMustBeOnTypeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing HarmonyPatch attribute on type.
        /// </summary>
        internal static string HarmonyPatchAttributeMustBeOnTypeTitle {
            get {
                return ResourceManager.GetString("HarmonyPatchAttributeMustBeOnTypeTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not enough information to target a method..
        /// </summary>
        internal static string MethodMustBeSpecifiedMessageFormat {
            get {
                return ResourceManager.GetString("MethodMustBeSpecifiedMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not enough information to target a method.
        /// </summary>
        internal static string MethodMustBeSpecifiedTitle {
            get {
                return ResourceManager.GetString("MethodMustBeSpecifiedTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Target method &apos;{0}&apos; does not exist in type &apos;{1}&apos;..
        /// </summary>
        internal static string MethodMustExistMessageFormat {
            get {
                return ResourceManager.GetString("MethodMustExistMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Target method does not exist.
        /// </summary>
        internal static string MethodMustExistTitle {
            get {
                return ResourceManager.GetString("MethodMustExistTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ambiguous match on target method &apos;{0}&apos; in type &apos;{1}&apos;: {2} candidates found..
        /// </summary>
        internal static string MethodMustNotBeAmbiguousMessageFormat {
            get {
                return ResourceManager.GetString("MethodMustNotBeAmbiguousMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ambiguous match on target method.
        /// </summary>
        internal static string MethodMustNotBeAmbiguousTitle {
            get {
                return ResourceManager.GetString("MethodMustNotBeAmbiguousTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Conflicting method specifications found..
        /// </summary>
        internal static string MethodMustNotBeOverspecifiedMessageFormat {
            get {
                return ResourceManager.GetString("MethodMustNotBeOverspecifiedMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Conflicting method specifications found.
        /// </summary>
        internal static string MethodMustNotBeOverspecifiedTitle {
            get {
                return ResourceManager.GetString("MethodMustNotBeOverspecifiedTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple method kinds are assigned to the patch method. Only one method kind will be recognized..
        /// </summary>
        internal static string PatchMethodMustHaveSingleKindMessageFormat {
            get {
                return ResourceManager.GetString("PatchMethodMustHaveSingleKindMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple method kinds are assigned to the patch method.
        /// </summary>
        internal static string PatchMethodMustHaveSingleKindTitle {
            get {
                return ResourceManager.GetString("PatchMethodMustHaveSingleKindTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Target type &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string TypeMustExistMessageFormat {
            get {
                return ResourceManager.GetString("TypeMustExistMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Target type does not exist.
        /// </summary>
        internal static string TypeMustExistTitle {
            get {
                return ResourceManager.GetString("TypeMustExistTitle", resourceCulture);
            }
        }
    }
}
