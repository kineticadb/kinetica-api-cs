/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createSchema(string,IDictionary{string, string})" />.
    /// <br />
    /// Creates a SQL-style <a href="../../../concepts/schemas/"
    /// target="_top">schema</a>. Schemas are containers for tables and views.
    /// Multiple tables and views can be defined with the same name in
    /// different schemas.</summary>
    public class CreateSchemaRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateSchemaRequest.Options.NO_ERROR_IF_EXISTS">NO_ERROR_IF_EXISTS</see>:</term>
        ///         <description>If <i>true</i>, prevents an error from
        /// occurring if the schema already exists.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>true</i>, prevents an error from occurring if
            /// the schema already exists.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateSchemaRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateSchemaRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateSchemaRequest.Options.FALSE">FALSE</see>.</summary>
            public const string NO_ERROR_IF_EXISTS = "no_error_if_exists";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Name of the schema to be created.  Has the same naming
        /// restrictions as <a href="../../../concepts/tables/"
        /// target="_top">tables</a>.  </summary>
        public string schema_name { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateSchemaRequest.Options.NO_ERROR_IF_EXISTS">NO_ERROR_IF_EXISTS</see>:</term>
        ///         <description>If <i>true</i>, prevents an error from
        /// occurring if the schema already exists.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateSchemaRequest object with default
        /// parameters.</summary>
        public CreateSchemaRequest() { }

        /// <summary>Constructs a CreateSchemaRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="schema_name">Name of the schema to be created.  Has
        /// the same naming restrictions as <a href="../../../concepts/tables/"
        /// target="_top">tables</a>.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateSchemaRequest.Options.NO_ERROR_IF_EXISTS">NO_ERROR_IF_EXISTS</see>:</term>
        ///         <description>If <i>true</i>, prevents an error from
        /// occurring if the schema already exists.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public CreateSchemaRequest( string schema_name,
                                    IDictionary<string, string> options = null)
        {
            this.schema_name = schema_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateSchemaRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createSchema(string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateSchemaResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="CreateSchemaRequest.schema_name"
        /// />.  </summary>
        public string schema_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateSchemaResponse




}  // end namespace kinetica
