/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.adminVerifyDb(IDictionary{string, string})" />.
    /// <br />
    /// Verify database is in a consistent state.  When inconsistencies or
    /// errors are found, the verified_ok flag in the response is set to false
    /// and the list of errors found is provided in the error_list.</summary>
    public class AdminVerifyDbRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.REBUILD_ON_ERROR">REBUILD_ON_ERROR</see>:</term>
        ///         <description>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.VERIFY_PERSIST">VERIFY_PERSIST</see>:</term>
        ///         <description>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AdminVerifyDbRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see>.</summary>
            public const string REBUILD_ON_ERROR = "rebuild_on_error";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AdminVerifyDbRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see>.</summary>
            public const string VERIFY_PERSIST = "verify_persist";
        } // end struct Options


        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.REBUILD_ON_ERROR">REBUILD_ON_ERROR</see>:</term>
        ///         <description>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.VERIFY_PERSIST">VERIFY_PERSIST</see>:</term>
        ///         <description>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AdminVerifyDbRequest object with default
        /// parameters.</summary>
        public AdminVerifyDbRequest() { }

        /// <summary>Constructs an AdminVerifyDbRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.REBUILD_ON_ERROR">REBUILD_ON_ERROR</see>:</term>
        ///         <description>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.VERIFY_PERSIST">VERIFY_PERSIST</see>:</term>
        ///         <description>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminVerifyDbRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public AdminVerifyDbRequest( IDictionary<string, string> options = null)
        {
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AdminVerifyDbRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.adminVerifyDb(IDictionary{string, string})"
    /// />.</summary>
    public class AdminVerifyDbResponse : KineticaData
    {

        /// <summary>True if no errors were found, false otherwise.  </summary>
        public bool verified_ok { get; set; } = false;

        /// <summary>List of errors found while validating the database
        /// internal state.  </summary>
        public IList<string> error_list { get; set; } = new List<string>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AdminVerifyDbResponse




}  // end namespace kinetica
