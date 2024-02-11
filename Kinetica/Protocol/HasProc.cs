/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.hasProc(HasProcRequest)">Kinetica.hasProc</see>.
    /// </summary>
    /// <remarks><para>Checks the existence of a proc with the given name.
    /// </para></remarks>
    public class HasProcRequest : KineticaData
    {
        /// <summary>Name of the proc to check for existence.</summary>
        public string proc_name { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a HasProcRequest object with default
        /// parameters.</summary>
        public HasProcRequest() { }

        /// <summary>Constructs a HasProcRequest object with the specified
        /// parameters.</summary>
        ///
        /// <param name="proc_name">Name of the proc to check for existence.
        /// </param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public HasProcRequest( string proc_name,
                               IDictionary<string, string> options = null)
        {
            this.proc_name = proc_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class HasProcRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.hasProc(HasProcRequest)">Kinetica.hasProc</see>.
    /// </summary>
    public class HasProcResponse : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="proc_exists" />.</summary>
        /// <remarks><para>Indicates whether the proc exists or not.</para>
        /// </remarks>
        public struct ProcExists
        {
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct ProcExists

        /// <summary>Value of <see
        /// cref="HasProcRequest.proc_name">proc_name</see></summary>
        public string proc_name { get; set; }

        /// <summary>Indicates whether the proc exists or not.</summary>
        /// <remarks><para>Supported values:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="ProcExists.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ProcExists.FALSE">FALSE</see></term>
        ///     </item>
        /// </list></remarks>
        public bool proc_exists { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class HasProcResponse
} // end namespace kinetica
