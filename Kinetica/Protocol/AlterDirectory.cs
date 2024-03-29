/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterDirectory(AlterDirectoryRequest)">Kinetica.alterDirectory</see>.
    /// </summary>
    /// <remarks><para>Alters an existing directory in <a
    /// href="../../../tools/kifs/" target="_top">KiFS</a>.</para></remarks>
    public class AlterDirectoryRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="directory_updates_map" />.</summary>
        /// <remarks><para>Map containing the properties of the directory to be
        /// altered. Error if empty.</para></remarks>
        public struct DirectoryUpdatesMap
        {
            /// <summary>The maximum capacity, in bytes, to apply to the
            /// directory.</summary>
            /// <remarks><para>Set to -1 to indicate no upper limit.</para>
            /// </remarks>
            public const string DATA_LIMIT = "data_limit";
        } // end struct DirectoryUpdatesMap

        /// <summary>Name of the directory in KiFS to be altered.</summary>
        public string directory_name { get; set; }

        /// <summary>Map containing the properties of the directory to be
        /// altered.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="DirectoryUpdatesMap.DATA_LIMIT">DATA_LIMIT</see>:
        ///         </term>
        ///         <description>The maximum capacity, in bytes, to apply to
        ///         the directory. Set to -1 to indicate no upper limit.
        ///         </description>
        ///     </item>
        /// </list></remarks>
        public IDictionary<string, string> directory_updates_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AlterDirectoryRequest object with default
        /// parameters.</summary>
        public AlterDirectoryRequest() { }

        /// <summary>Constructs an AlterDirectoryRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="directory_name">Name of the directory in KiFS to be
        /// altered.</param>
        /// <param name="directory_updates_map">Map containing the properties
        /// of the directory to be altered. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="DirectoryUpdatesMap.DATA_LIMIT">DATA_LIMIT</see>:
        ///         </term>
        ///         <description>The maximum capacity, in bytes, to apply to
        ///         the directory. Set to -1 to indicate no upper limit.
        ///         </description>
        ///     </item>
        /// </list></param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public AlterDirectoryRequest( string directory_name,
                                      IDictionary<string, string> directory_updates_map,
                                      IDictionary<string, string> options = null)
        {
            this.directory_name = directory_name ?? "";
            this.directory_updates_map = directory_updates_map ?? new Dictionary<string, string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AlterDirectoryRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterDirectory(AlterDirectoryRequest)">Kinetica.alterDirectory</see>.
    /// </summary>
    public class AlterDirectoryResponse : KineticaData
    {
        /// <summary>Value of <see
        /// cref="AlterDirectoryRequest.directory_name">directory_name</see>.
        /// </summary>
        public string directory_name { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AlterDirectoryResponse
} // end namespace kinetica
