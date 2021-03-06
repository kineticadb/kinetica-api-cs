/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterTier(string,IDictionary{string, string})" />.
    /// <br />
    /// Alters properties of an exisiting
    /// <a href="../../../rm/concepts/#storage-tiers" target="_top">tier</a> to
    /// facilitate
    /// <a href="../../../rm/concepts/" target="_top">resource management</a>.
    /// <br />
    /// To disable
    /// <a href="../../../rm/concepts/#watermark-based-eviction"
    /// target="_top">watermark-based eviction</a>,
    /// set both <i>high_watermark</i> and
    /// <i>low_watermark</i> to 100.</summary>
    public class AlterTierRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterTierRequest.Options.CAPACITY">CAPACITY</see>:</term>
        ///         <description>Maximum size in bytes this tier may hold at
        /// once.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterTierRequest.Options.HIGH_WATERMARK">HIGH_WATERMARK</see>:</term>
        ///         <description>Threshold of usage of this tier's resource
        /// that, once exceeded, will trigger watermark-based eviction from
        /// this tier.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterTierRequest.Options.LOW_WATERMARK">LOW_WATERMARK</see>:</term>
        ///         <description>Threshold of resource usage that, once fallen
        /// below after crossing the <i>high_watermark</i>, will cease
        /// watermark-based eviction from this tier.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Maximum size in bytes this tier may hold at
            /// once.</summary>
            public const string CAPACITY = "capacity";

            /// <summary>Threshold of usage of this tier's resource that, once
            /// exceeded, will trigger watermark-based eviction from this
            /// tier.</summary>
            public const string HIGH_WATERMARK = "high_watermark";

            /// <summary>Threshold of resource usage that, once fallen below
            /// after crossing the <i>high_watermark</i>, will cease
            /// watermark-based eviction from this tier.</summary>
            public const string LOW_WATERMARK = "low_watermark";
        } // end struct Options


        /// <summary>Name of the tier to be altered. Must be an existing tier
        /// group name.  </summary>
        public string name { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterTierRequest.Options.CAPACITY">CAPACITY</see>:</term>
        ///         <description>Maximum size in bytes this tier may hold at
        /// once.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterTierRequest.Options.HIGH_WATERMARK">HIGH_WATERMARK</see>:</term>
        ///         <description>Threshold of usage of this tier's resource
        /// that, once exceeded, will trigger watermark-based eviction from
        /// this tier.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterTierRequest.Options.LOW_WATERMARK">LOW_WATERMARK</see>:</term>
        ///         <description>Threshold of resource usage that, once fallen
        /// below after crossing the <i>high_watermark</i>, will cease
        /// watermark-based eviction from this tier.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AlterTierRequest object with default
        /// parameters.</summary>
        public AlterTierRequest() { }

        /// <summary>Constructs an AlterTierRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="name">Name of the tier to be altered. Must be an
        /// existing tier group name.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterTierRequest.Options.CAPACITY">CAPACITY</see>:</term>
        ///         <description>Maximum size in bytes this tier may hold at
        /// once.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterTierRequest.Options.HIGH_WATERMARK">HIGH_WATERMARK</see>:</term>
        ///         <description>Threshold of usage of this tier's resource
        /// that, once exceeded, will trigger watermark-based eviction from
        /// this tier.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterTierRequest.Options.LOW_WATERMARK">LOW_WATERMARK</see>:</term>
        ///         <description>Threshold of resource usage that, once fallen
        /// below after crossing the <i>high_watermark</i>, will cease
        /// watermark-based eviction from this tier.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public AlterTierRequest( string name,
                                 IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AlterTierRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterTier(string,IDictionary{string, string})"
    /// />.</summary>
    public class AlterTierResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="AlterTierRequest.name" />.
        /// </summary>
        public string name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AlterTierResponse




}  // end namespace kinetica
