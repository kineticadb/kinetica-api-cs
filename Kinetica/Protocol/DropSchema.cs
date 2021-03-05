/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.dropSchema(string,IDictionary{string, string})" />.
    /// <br />
    /// Drops an existing SQL-style <a href="../../../concepts/schemas/"
    /// target="_top">schema</a>, specified in <see cref="schema_name"
    /// />.</summary>
    public class DropSchemaRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>true</i> and if the schema specified in
        /// <paramref cref="DropSchemaRequest.schema_name" /> does not exist,
        /// no error is returned. If <i>false</i> and if the schema specified
        /// in <paramref cref="DropSchemaRequest.schema_name" /> does not
        /// exist, then an error is returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.CASCADE">CASCADE</see>:</term>
        ///         <description>If <i>true</i>, all tables within the schema
        /// will be dropped. If <i>false</i>, the schema will be dropped only
        /// if empty.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>true</i> and if the schema specified in <see
            /// cref="schema_name" /> does not exist, no error is returned. If
            /// <i>false</i> and if the schema specified in <see
            /// cref="schema_name" /> does not exist, then an error is
            /// returned.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="DropSchemaRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="DropSchemaRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="DropSchemaRequest.Options.FALSE">FALSE</see>.</summary>
            public const string NO_ERROR_IF_NOT_EXISTS = "no_error_if_not_exists";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>If <i>true</i>, all tables within the schema will be
            /// dropped. If <i>false</i>, the schema will be dropped only if
            /// empty.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="DropSchemaRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="DropSchemaRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="DropSchemaRequest.Options.FALSE">FALSE</see>.</summary>
            public const string CASCADE = "cascade";
        } // end struct Options


        /// <summary>Name of the schema to be dropped. Must be an existing
        /// schema.  </summary>
        public string schema_name { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>true</i> and if the schema specified in
        /// <paramref cref="DropSchemaRequest.schema_name" /> does not exist,
        /// no error is returned. If <i>false</i> and if the schema specified
        /// in <paramref cref="DropSchemaRequest.schema_name" /> does not
        /// exist, then an error is returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.CASCADE">CASCADE</see>:</term>
        ///         <description>If <i>true</i>, all tables within the schema
        /// will be dropped. If <i>false</i>, the schema will be dropped only
        /// if empty.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a DropSchemaRequest object with default
        /// parameters.</summary>
        public DropSchemaRequest() { }

        /// <summary>Constructs a DropSchemaRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="schema_name">Name of the schema to be dropped. Must be
        /// an existing schema.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>true</i> and if the schema specified in
        /// <paramref cref="DropSchemaRequest.schema_name" /> does not exist,
        /// no error is returned. If <i>false</i> and if the schema specified
        /// in <paramref cref="DropSchemaRequest.schema_name" /> does not
        /// exist, then an error is returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.CASCADE">CASCADE</see>:</term>
        ///         <description>If <i>true</i>, all tables within the schema
        /// will be dropped. If <i>false</i>, the schema will be dropped only
        /// if empty.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="DropSchemaRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public DropSchemaRequest( string schema_name,
                                  IDictionary<string, string> options = null)
        {
            this.schema_name = schema_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class DropSchemaRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.dropSchema(string,IDictionary{string, string})"
    /// />.</summary>
    public class DropSchemaResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="DropSchemaRequest.schema_name"
        /// />.  </summary>
        public string schema_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class DropSchemaResponse




}  // end namespace kinetica
