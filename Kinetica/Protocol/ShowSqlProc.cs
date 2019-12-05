/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showSqlProc(string,IDictionary{string, string})" />.
    /// <br />
    /// Procedures</summary>
    public class ShowSqlProcRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSqlProcRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>false</i> will return an error if the
        /// provided  does not exist. If <i>true</i> then it will return an
        /// empty result.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSqlProcRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowSqlProcRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowSqlProcRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>false</i> will return an error if the provided
            /// does not exist. If <i>true</i> then it will return an empty
            /// result.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="ShowSqlProcRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="ShowSqlProcRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="ShowSqlProcRequest.Options.FALSE">FALSE</see>.</summary>
            public const string NO_ERROR_IF_NOT_EXISTS = "no_error_if_not_exists";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Name of the procedure for which to retrieve the
        /// information. If blank, then information about all procedures is
        /// returned.  The default value is ''.</summary>
        public string procedure_name { get; set; } = "";

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSqlProcRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>false</i> will return an error if the
        /// provided  does not exist. If <i>true</i> then it will return an
        /// empty result.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSqlProcRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowSqlProcRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowSqlProcRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowSqlProcRequest object with default
        /// parameters.</summary>
        public ShowSqlProcRequest() { }

        /// <summary>Constructs a ShowSqlProcRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="procedure_name">Name of the procedure for which to
        /// retrieve the information. If blank, then information about all
        /// procedures is returned.  The default value is ''.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSqlProcRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>false</i> will return an error if the
        /// provided  does not exist. If <i>true</i> then it will return an
        /// empty result.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSqlProcRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowSqlProcRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowSqlProcRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public ShowSqlProcRequest( string procedure_name = null,
                                   IDictionary<string, string> options = null)
        {
            this.procedure_name = procedure_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowSqlProcRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showSqlProc(string,IDictionary{string, string})"
    /// />.</summary>
    public class ShowSqlProcResponse : KineticaData
    {

        /// <summary>Additional information about the respective tables in
        /// @{procedure_names}.
        /// Supported values:
        /// <list type="bullet">
        /// </list>
        /// A set of string constants for the parameter <member
        /// name="additional_info" />.</summary>
        public struct AdditionalInfo
        {

            /// <summary>for sql procedures the current intial datetime string
            /// that periodic execution began.  The default value is
            /// ''.</summary>
            public const string EXECUTE_START_TIME = "execute_start_time";

            /// <summary>for sql procedures the current periodic execution
            /// interval in seconds.  The default value is ''.</summary>
            public const string EXECUTE_INTERVAL = "execute_interval";
        } // end struct AdditionalInfo


        /// <summary>Value of .  </summary>
        public IList<string> procedure_names { get; set; } = new List<string>();

        /// <summary>procedures  </summary>
        public IList<string> procedure_definitions { get; set; } = new List<string>();

        /// <summary>Additional information about the respective tables in
        /// @{procedure_names}.
        /// Supported values:
        /// <list type="bullet">
        /// </list>  </summary>
        public IList<IDictionary<string, string>> additional_info { get; set; } = new List<IDictionary<string, string>>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowSqlProcResponse




}  // end namespace kinetica
