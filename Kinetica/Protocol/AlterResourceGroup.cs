/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterResourceGroup(string,IDictionary{string, IDictionary{string, string}},string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Alters the properties of an exisiting resource group to facilitate
    /// resource management.</summary>
    public class AlterResourceGroupRequest : KineticaData
    {

        /// <summary>Optional map containing tier names and their respective
        /// attribute group limits.  The only valid attribute limit that can be
        /// set is max_memory (in bytes) for the VRAM & RAM tiers.
        /// <br />
        /// For instance, to set max VRAM capacity to 1GB and max RAM capacity
        /// to 10GB, use:  {'VRAM':{'max_memory':'1000000000'},
        /// 'RAM':{'max_memory':'10000000000'}}
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.TierAttributes.MAX_MEMORY">MAX_MEMORY</see>:</term>
        ///         <description>Maximum amount of memory usable in the given
        /// tier at one time for this group.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see
        /// cref="tier_attributes" />.</summary>
        public struct TierAttributes
        {

            /// <summary>Maximum amount of memory usable in the given tier at
            /// one time for this group.</summary>
            public const string MAX_MEMORY = "max_memory";
        } // end struct TierAttributes


        /// <summary>If the resource group ranking is to be updated, this
        /// indicates the relative ranking among existing resource groups where
        /// this resource group will be moved; leave blank if not changing the
        /// ranking.  When using <i>before</i> or <i>after</i>, specify which
        /// resource group this one will be inserted before or after in
        /// <paramref cref="AlterResourceGroupRequest.adjoining_resource_group"
        /// />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.EMPTY_STRING">EMPTY_STRING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.FIRST">FIRST</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.LAST">LAST</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.BEFORE">BEFORE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.AFTER">AFTER</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterResourceGroupRequest.Ranking.EMPTY_STRING">EMPTY_STRING</see>.
        /// A set of string constants for the parameter <see cref="ranking"
        /// />.</summary>
        public struct Ranking
        {
            public const string EMPTY_STRING = "";
            public const string FIRST = "first";
            public const string LAST = "last";
            public const string BEFORE = "before";
            public const string AFTER = "after";
        } // end struct Ranking


        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.MAX_CPU_CONCURRENCY">MAX_CPU_CONCURRENCY</see>:</term>
        ///         <description>Maximum number of simultaneous threads that
        /// will be used to execute a request for this group.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.MAX_SCHEDULING_PRIORITY">MAX_SCHEDULING_PRIORITY</see>:</term>
        ///         <description>Maximum priority of a scheduled task for this
        /// group.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.MAX_TIER_PRIORITY">MAX_TIER_PRIORITY</see>:</term>
        ///         <description>Maximum priority of a tiered object for this
        /// group.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.IS_DEFAULT_GROUP">IS_DEFAULT_GROUP</see>:</term>
        ///         <description>If <i>true</i>, this request applies to the
        /// global default resource group. It is an error for this field to be
        /// <i>true</i> when the <paramref
        /// cref="AlterResourceGroupRequest.name" /> field is also populated.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterResourceGroupRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Maximum number of simultaneous threads that will be
            /// used to execute a request for this group.</summary>
            public const string MAX_CPU_CONCURRENCY = "max_cpu_concurrency";

            /// <summary>Maximum priority of a scheduled task for this
            /// group.</summary>
            public const string MAX_SCHEDULING_PRIORITY = "max_scheduling_priority";

            /// <summary>Maximum priority of a tiered object for this
            /// group.</summary>
            public const string MAX_TIER_PRIORITY = "max_tier_priority";

            /// <summary>If <i>true</i>, this request applies to the global
            /// default resource group. It is an error for this field to be
            /// <i>true</i> when the <see cref="name" /> field is also
            /// populated.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AlterResourceGroupRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterResourceGroupRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AlterResourceGroupRequest.Options.FALSE">FALSE</see>.</summary>
            public const string IS_DEFAULT_GROUP = "is_default_group";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Name of the group to be altered. Must be an existing
        /// resource group name.  </summary>
        public string name { get; set; }

        /// <summary>Optional map containing tier names and their respective
        /// attribute group limits.  The only valid attribute limit that can be
        /// set is max_memory (in bytes) for the VRAM & RAM tiers.
        /// <br />
        /// For instance, to set max VRAM capacity to 1GB and max RAM capacity
        /// to 10GB, use:  {'VRAM':{'max_memory':'1000000000'},
        /// 'RAM':{'max_memory':'10000000000'}}
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.TierAttributes.MAX_MEMORY">MAX_MEMORY</see>:</term>
        ///         <description>Maximum amount of memory usable in the given
        /// tier at one time for this group.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, IDictionary<string, string>> tier_attributes { get; set; } = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>If the resource group ranking is to be updated, this
        /// indicates the relative ranking among existing resource groups where
        /// this resource group will be moved; leave blank if not changing the
        /// ranking.  When using <i>before</i> or <i>after</i>, specify which
        /// resource group this one will be inserted before or after in
        /// <paramref cref="AlterResourceGroupRequest.adjoining_resource_group"
        /// />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.EMPTY_STRING">EMPTY_STRING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.FIRST">FIRST</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.LAST">LAST</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.BEFORE">BEFORE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.AFTER">AFTER</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterResourceGroupRequest.Ranking.EMPTY_STRING">EMPTY_STRING</see>.
        /// </summary>
        public string ranking { get; set; } = Ranking.EMPTY_STRING;

        /// <summary>If <paramref cref="AlterResourceGroupRequest.ranking" />
        /// is <i>before</i> or <i>after</i>, this field indicates the resource
        /// group before or after which the current group will be placed;
        /// otherwise, leave blank.  The default value is ''.</summary>
        public string adjoining_resource_group { get; set; } = "";

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.MAX_CPU_CONCURRENCY">MAX_CPU_CONCURRENCY</see>:</term>
        ///         <description>Maximum number of simultaneous threads that
        /// will be used to execute a request for this group.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.MAX_SCHEDULING_PRIORITY">MAX_SCHEDULING_PRIORITY</see>:</term>
        ///         <description>Maximum priority of a scheduled task for this
        /// group.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.MAX_TIER_PRIORITY">MAX_TIER_PRIORITY</see>:</term>
        ///         <description>Maximum priority of a tiered object for this
        /// group.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.IS_DEFAULT_GROUP">IS_DEFAULT_GROUP</see>:</term>
        ///         <description>If <i>true</i>, this request applies to the
        /// global default resource group. It is an error for this field to be
        /// <i>true</i> when the <paramref
        /// cref="AlterResourceGroupRequest.name" /> field is also populated.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterResourceGroupRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AlterResourceGroupRequest object with
        /// default parameters.</summary>
        public AlterResourceGroupRequest() { }

        /// <summary>Constructs an AlterResourceGroupRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="name">Name of the group to be altered. Must be an
        /// existing resource group name.  </param>
        /// <param name="tier_attributes">Optional map containing tier names
        /// and their respective attribute group limits.  The only valid
        /// attribute limit that can be set is max_memory (in bytes) for the
        /// VRAM & RAM tiers.
        /// For instance, to set max VRAM capacity to 1GB and max RAM capacity
        /// to 10GB, use:  {'VRAM':{'max_memory':'1000000000'},
        /// 'RAM':{'max_memory':'10000000000'}}
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.TierAttributes.MAX_MEMORY">MAX_MEMORY</see>:</term>
        ///         <description>Maximum amount of memory usable in the given
        /// tier at one time for this group.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// <param name="ranking">If the resource group ranking is to be
        /// updated, this indicates the relative ranking among existing
        /// resource groups where this resource group will be moved; leave
        /// blank if not changing the ranking.  When using <i>before</i> or
        /// <i>after</i>, specify which resource group this one will be
        /// inserted before or after in <paramref
        /// cref="AlterResourceGroupRequest.adjoining_resource_group" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.EMPTY_STRING">EMPTY_STRING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.FIRST">FIRST</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.LAST">LAST</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.BEFORE">BEFORE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Ranking.AFTER">AFTER</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterResourceGroupRequest.Ranking.EMPTY_STRING">EMPTY_STRING</see>.
        /// </param>
        /// <param name="adjoining_resource_group">If <paramref
        /// cref="AlterResourceGroupRequest.ranking" /> is <i>before</i> or
        /// <i>after</i>, this field indicates the resource group before or
        /// after which the current group will be placed; otherwise, leave
        /// blank.  The default value is ''.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.MAX_CPU_CONCURRENCY">MAX_CPU_CONCURRENCY</see>:</term>
        ///         <description>Maximum number of simultaneous threads that
        /// will be used to execute a request for this group.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.MAX_SCHEDULING_PRIORITY">MAX_SCHEDULING_PRIORITY</see>:</term>
        ///         <description>Maximum priority of a scheduled task for this
        /// group.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.MAX_TIER_PRIORITY">MAX_TIER_PRIORITY</see>:</term>
        ///         <description>Maximum priority of a tiered object for this
        /// group.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.IS_DEFAULT_GROUP">IS_DEFAULT_GROUP</see>:</term>
        ///         <description>If <i>true</i>, this request applies to the
        /// global default resource group. It is an error for this field to be
        /// <i>true</i> when the <paramref
        /// cref="AlterResourceGroupRequest.name" /> field is also populated.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterResourceGroupRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterResourceGroupRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public AlterResourceGroupRequest( string name,
                                          IDictionary<string, IDictionary<string, string>> tier_attributes = null,
                                          string ranking = null,
                                          string adjoining_resource_group = null,
                                          IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.tier_attributes = tier_attributes ?? new Dictionary<string, IDictionary<string, string>>();
            this.ranking = ranking ?? Ranking.EMPTY_STRING;
            this.adjoining_resource_group = adjoining_resource_group ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AlterResourceGroupRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterResourceGroup(string,IDictionary{string, IDictionary{string, string}},string,string,IDictionary{string, string})"
    /// />.</summary>
    public class AlterResourceGroupResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="AlterResourceGroupRequest.name"
        /// />.  </summary>
        public string name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AlterResourceGroupResponse




}  // end namespace kinetica
