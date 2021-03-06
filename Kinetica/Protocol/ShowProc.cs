/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showProc(string,IDictionary{string, string})" />.
    /// <br />
    /// Shows information about a proc.</summary>
    public class ShowProcRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcRequest.Options.INCLUDE_FILES">INCLUDE_FILES</see>:</term>
        ///         <description>If set to <i>true</i>, the files that make up
        /// the proc will be returned. If set to <i>false</i>, the files will
        /// not be returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowProcRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If set to <i>true</i>, the files that make up the proc
            /// will be returned. If set to <i>false</i>, the files will not be
            /// returned.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="ShowProcRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="ShowProcRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="ShowProcRequest.Options.FALSE">FALSE</see>.</summary>
            public const string INCLUDE_FILES = "include_files";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Name of the proc to show information about. If specified,
        /// must be the name of a currently existing proc. If not specified,
        /// information about all procs will be returned.  The default value is
        /// ''.</summary>
        public string proc_name { get; set; } = "";

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcRequest.Options.INCLUDE_FILES">INCLUDE_FILES</see>:</term>
        ///         <description>If set to <i>true</i>, the files that make up
        /// the proc will be returned. If set to <i>false</i>, the files will
        /// not be returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowProcRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowProcRequest object with default
        /// parameters.</summary>
        public ShowProcRequest() { }

        /// <summary>Constructs a ShowProcRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="proc_name">Name of the proc to show information about.
        /// If specified, must be the name of a currently existing proc. If not
        /// specified, information about all procs will be returned.  The
        /// default value is ''.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcRequest.Options.INCLUDE_FILES">INCLUDE_FILES</see>:</term>
        ///         <description>If set to <i>true</i>, the files that make up
        /// the proc will be returned. If set to <i>false</i>, the files will
        /// not be returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowProcRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public ShowProcRequest( string proc_name = null,
                                IDictionary<string, string> options = null)
        {
            this.proc_name = proc_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowProcRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showProc(string,IDictionary{string, string})"
    /// />.</summary>
    public class ShowProcResponse : KineticaData
    {

        /// <summary>The execution modes of the procs named in <member
        /// name="proc_names" />.
        /// Supported values:
        /// <list type="bullet">
        /// </list>
        /// A set of string constants for the parameter <member
        /// name="execution_modes" />.</summary>
        public struct ExecutionModes
        {

            /// <summary>Distributed</summary>
            public const string DISTRIBUTED = "distributed";

            /// <summary>Nondistributed</summary>
            public const string NONDISTRIBUTED = "nondistributed";
        } // end struct ExecutionModes


        /// <summary>The proc names.  </summary>
        public IList<string> proc_names { get; set; } = new List<string>();

        /// <summary>The execution modes of the procs named in <member
        /// name="proc_names" />.
        /// Supported values:
        /// <list type="bullet">
        /// </list>  </summary>
        public IList<string> execution_modes { get; set; } = new List<string>();

        /// <summary>Maps of the files that make up the procs named in <member
        /// name="proc_names" />.  </summary>
        public IList<IDictionary<string, byte[]>> files { get; set; } = new List<IDictionary<string, byte[]>>();

        /// <summary>The commands (excluding arguments) that will be invoked
        /// when the procs named in <member name="proc_names" /> are executed.
        /// </summary>
        public IList<string> commands { get; set; } = new List<string>();

        /// <summary>Arrays of command-line arguments that will be passed to
        /// the procs named in <member name="proc_names" /> when executed.
        /// </summary>
        public IList<IList<string>> args { get; set; } = new List<IList<string>>();

        /// <summary>The optional parameters for the procs named in <member
        /// name="proc_names" />.  </summary>
        public IList<IDictionary<string, string>> options { get; set; } = new List<IDictionary<string, string>>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowProcResponse




}  // end namespace kinetica
