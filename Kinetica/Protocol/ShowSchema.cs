/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showSchema(string,IDictionary{string, string})" />.
    /// <br />
    /// Retrieves information about a <a href="../../../concepts/schemas/"
    /// target="_top">schema</a> (or all schemas), as specified in <see
    /// cref="schema_name" />.</summary>
    public class ShowSchemaRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSchemaRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>false</i> will return an error if the
        /// provided <paramref cref="ShowSchemaRequest.schema_name" /> does not
        /// exist. If <i>true</i> then it will return an empty result if the
        /// provided <paramref cref="ShowSchemaRequest.schema_name" /> does not
        /// exist.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>false</i> will return an error if the provided
            /// <see cref="schema_name" /> does not exist. If <i>true</i> then
            /// it will return an empty result if the provided <see
            /// cref="schema_name" /> does not exist.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="ShowSchemaRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="ShowSchemaRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="ShowSchemaRequest.Options.FALSE">FALSE</see>.</summary>
            public const string NO_ERROR_IF_NOT_EXISTS = "no_error_if_not_exists";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Name of the schema for which to retrieve the information.
        /// If blank, then info for all schemas is returned.  </summary>
        public string schema_name { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSchemaRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>false</i> will return an error if the
        /// provided <paramref cref="ShowSchemaRequest.schema_name" /> does not
        /// exist. If <i>true</i> then it will return an empty result if the
        /// provided <paramref cref="ShowSchemaRequest.schema_name" /> does not
        /// exist.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowSchemaRequest object with default
        /// parameters.</summary>
        public ShowSchemaRequest() { }

        /// <summary>Constructs a ShowSchemaRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="schema_name">Name of the schema for which to retrieve
        /// the information. If blank, then info for all schemas is returned.
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSchemaRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>false</i> will return an error if the
        /// provided <paramref cref="ShowSchemaRequest.schema_name" /> does not
        /// exist. If <i>true</i> then it will return an empty result if the
        /// provided <paramref cref="ShowSchemaRequest.schema_name" /> does not
        /// exist.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public ShowSchemaRequest( string schema_name,
                                  IDictionary<string, string> options = null)
        {
            this.schema_name = schema_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowSchemaRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showSchema(string,IDictionary{string, string})"
    /// />.</summary>
    public class ShowSchemaResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="ShowSchemaRequest.schema_name"
        /// />.  </summary>
        public string schema_name { get; set; }

        /// <summary>A list of all schema names for which information is
        /// returned  </summary>
        public IList<string> schema_names { get; set; } = new List<string>();

        /// <summary>An array of arrays containing a list of tables in each of
        /// the respective <member name="schema_names" />.  </summary>
        public IList<IList<string>> schema_tables { get; set; } = new List<IList<string>>();

        /// <summary>Additional information about the respective tables in
        /// <member name="schema_names" />.  </summary>
        public IList<IDictionary<string, string>> additional_info { get; set; } = new List<IDictionary<string, string>>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowSchemaResponse




}  // end namespace kinetica
