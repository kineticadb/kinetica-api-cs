/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showEnvironment(ShowEnvironmentRequest)">Kinetica.showEnvironment</see>.
    /// </summary>
    /// <remarks><para>Shows information about a specified <a
    /// href="../../../concepts/udf/" target="_top">user-defined function</a>
    /// (UDF) environment or all environments.
    /// Returns detailed information about existing environments.</para>
    /// </remarks>
    public class ShowEnvironmentRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>If <see cref="Options.TRUE">TRUE</see> and if the
            /// environment specified in <see cref="environment_name" /> does
            /// not exist, no error is returned.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// <para>The default value is <see
            /// cref="Options.FALSE">FALSE</see>.</para></remarks>
            public const string NO_ERROR_IF_NOT_EXISTS = "no_error_if_not_exists";

            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options

        /// <summary>Name of the environment on which to retrieve information.
        /// </summary>
        /// <remarks><para>The name must refer to a currently existing
        /// environment. If '*' or an empty value is specified, information
        /// about all environments will be returned. The default value is ''.
        /// </para></remarks>
        public string environment_name { get; set; } = "";

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> and if
        ///         the environment specified in <see cref="environment_name"
        ///         /> does not exist, no error is returned. If <see
        ///         cref="Options.FALSE">FALSE</see> and if the environment
        ///         specified in <see cref="environment_name" /> does not
        ///         exist, then an error is returned.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a ShowEnvironmentRequest object with default
        /// parameters.</summary>
        public ShowEnvironmentRequest() { }

        /// <summary>Constructs a ShowEnvironmentRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="environment_name">Name of the environment on which to
        /// retrieve information. The name must refer to a currently existing
        /// environment. If '*' or an empty value is specified, information
        /// about all environments will be returned. The default value is ''.
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> and if
        ///         the environment specified in <paramref
        ///         name="environment_name" /> does not exist, no error is
        ///         returned. If <see cref="Options.FALSE">FALSE</see> and if
        ///         the environment specified in <paramref
        ///         name="environment_name" /> does not exist, then an error is
        ///         returned.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public ShowEnvironmentRequest( string environment_name = null,
                                       IDictionary<string, string> options = null)
        {
            this.environment_name = environment_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class ShowEnvironmentRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showEnvironment(ShowEnvironmentRequest)">Kinetica.showEnvironment</see>.
    /// </summary>
    public class ShowEnvironmentResponse : KineticaData
    {
        /// <summary>A list of all credential names.</summary>
        public IList<string> environment_names { get; set; } = new List<string>();

        /// <summary>Information about the installed packages in the respective
        /// environments in <see cref="environment_names" />.</summary>
        public IList<IList<string>> packages { get; set; } = new List<IList<string>>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class ShowEnvironmentResponse
} // end namespace kinetica
