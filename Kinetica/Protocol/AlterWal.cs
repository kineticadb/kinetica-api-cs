/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterWal(AlterWalRequest)">Kinetica.alterWal</see>.
    /// </summary>
    /// <remarks><para>Alters table wal settings.
    /// Returns information about the requested table wal modifications.</para>
    /// </remarks>
    public class AlterWalRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>Maximum size of an individual segment file</summary>
            public const string MAX_SEGMENT_SIZE = "max_segment_size";

            /// <summary>Approximate number of segment files to split the wal
            /// across.</summary>
            /// <remarks><para>Must be at least two.</para></remarks>
            public const string SEGMENT_COUNT = "segment_count";

            /// <summary>Maximum size of an individual segment file.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.NONE">NONE</see>:</term>
            ///         <description>Disables the wal</description>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.BACKGROUND">BACKGROUND</see>:
            ///         </term>
            ///         <description>Wal entries are periodically written
            ///         instead of immediately after each operation
            ///         </description>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FLUSH">FLUSH</see>:</term>
            ///         <description>Protects entries in the event of a
            ///         database crash</description>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FSYNC">FSYNC</see>:</term>
            ///         <description>Protects entries in the event of an OS
            ///         crash</description>
            ///     </item>
            /// </list></remarks>
            public const string SYNC_POLICY = "sync_policy";

            /// <summary>Disables the wal</summary>
            public const string NONE = "none";

            /// <summary>Wal entries are periodically written instead of
            /// immediately after each operation</summary>
            public const string BACKGROUND = "background";

            /// <summary>Protects entries in the event of a database crash
            /// </summary>
            public const string FLUSH = "flush";

            /// <summary>Protects entries in the event of an OS crash</summary>
            public const string FSYNC = "fsync";

            /// <summary>Specifies how frequently wal entries are written with
            /// background sync.</summary>
            /// <remarks><para>This is a global setting and can only be used
            /// with the system {options.table_names} specifier '*'.</para>
            /// </remarks>
            public const string FLUSH_FREQUENCY = "flush_frequency";

            /// <summary>If <see cref="Options.TRUE">TRUE</see> each entry will
            /// be checked against a protective checksum.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// <para>The default value is <see cref="Options.TRUE">TRUE</see>.
            /// </para></remarks>
            public const string CHECKSUM = "checksum";

            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>If <see cref="Options.TRUE">TRUE</see> tables with
            /// unique wal settings will be overridden when applying a system
            /// level change.</summary>
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
            public const string OVERRIDE_NON_DEFAULT = "override_non_default";

            /// <summary>If <see cref="Options.TRUE">TRUE</see> tables with
            /// unique wal settings will be reverted to the current global
            /// settings.</summary>
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
            public const string RESTORE_SYSTEM_SETTINGS = "restore_system_settings";

            /// <summary>If <see cref="Options.TRUE">TRUE</see> and a
            /// system-level change was requested, the system configuration
            /// will be written to disk upon successful application of this
            /// request.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// <para>The default value is <see cref="Options.TRUE">TRUE</see>.
            /// </para></remarks>
            public const string PERSIST = "persist";
        } // end struct Options

        /// <summary>List of tables to modify.</summary>
        /// <remarks><para>An asterisk changes the system settings.</para>
        /// </remarks>
        public IList<string> table_names { get; set; } = new List<string>();

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.MAX_SEGMENT_SIZE">MAX_SEGMENT_SIZE</see>:
        ///         </term>
        ///         <description>Maximum size of an individual segment file
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.SEGMENT_COUNT">SEGMENT_COUNT</see>:</term>
        ///         <description>Approximate number of segment files to split
        ///         the wal across. Must be at least two.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.SYNC_POLICY">SYNC_POLICY</see>:
        ///         </term>
        ///         <description>Maximum size of an individual segment file.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.NONE">NONE</see>:</term>
        ///                 <description>Disables the wal</description>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="Options.BACKGROUND">BACKGROUND</see>:</term>
        ///                 <description>Wal entries are periodically written
        ///                 instead of immediately after each operation
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FLUSH">FLUSH</see>:</term>
        ///                 <description>Protects entries in the event of a
        ///                 database crash</description>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FSYNC">FSYNC</see>:</term>
        ///                 <description>Protects entries in the event of an OS
        ///                 crash</description>
        ///             </item>
        ///         </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.FLUSH_FREQUENCY">FLUSH_FREQUENCY</see>:
        ///         </term>
        ///         <description>Specifies how frequently wal entries are
        ///         written with background sync. This is a global setting and
        ///         can only be used with the system {options.table_names}
        ///         specifier '*'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.CHECKSUM">CHECKSUM</see>:</term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> each
        ///         entry will be checked against a protective checksum.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.TRUE">TRUE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.OVERRIDE_NON_DEFAULT">OVERRIDE_NON_DEFAULT</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> tables
        ///         with unique wal settings will be overridden when applying a
        ///         system level change.
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
        ///     <item>
        ///         <term><see
        ///         cref="Options.RESTORE_SYSTEM_SETTINGS">RESTORE_SYSTEM_SETTINGS</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> tables
        ///         with unique wal settings will be reverted to the current
        ///         global settings. Cannot be used in conjunction with any
        ///         other option.
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
        ///     <item>
        ///         <term><see cref="Options.PERSIST">PERSIST</see>:</term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> and a
        ///         system-level change was requested, the system configuration
        ///         will be written to disk upon successful application of this
        ///         request. This will commit the changes from this request and
        ///         any additional in-memory modifications.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.TRUE">TRUE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AlterWalRequest object with default
        /// parameters.</summary>
        public AlterWalRequest() { }

        /// <summary>Constructs an AlterWalRequest object with the specified
        /// parameters.</summary>
        ///
        /// <param name="table_names">List of tables to modify. An asterisk
        /// changes the system settings.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.MAX_SEGMENT_SIZE">MAX_SEGMENT_SIZE</see>:
        ///         </term>
        ///         <description>Maximum size of an individual segment file
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.SEGMENT_COUNT">SEGMENT_COUNT</see>:</term>
        ///         <description>Approximate number of segment files to split
        ///         the wal across. Must be at least two.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.SYNC_POLICY">SYNC_POLICY</see>:
        ///         </term>
        ///         <description>Maximum size of an individual segment file.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.NONE">NONE</see>:</term>
        ///                 <description>Disables the wal</description>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="Options.BACKGROUND">BACKGROUND</see>:</term>
        ///                 <description>Wal entries are periodically written
        ///                 instead of immediately after each operation
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FLUSH">FLUSH</see>:</term>
        ///                 <description>Protects entries in the event of a
        ///                 database crash</description>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FSYNC">FSYNC</see>:</term>
        ///                 <description>Protects entries in the event of an OS
        ///                 crash</description>
        ///             </item>
        ///         </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.FLUSH_FREQUENCY">FLUSH_FREQUENCY</see>:
        ///         </term>
        ///         <description>Specifies how frequently wal entries are
        ///         written with background sync. This is a global setting and
        ///         can only be used with the system {options.table_names}
        ///         specifier '*'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.CHECKSUM">CHECKSUM</see>:</term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> each
        ///         entry will be checked against a protective checksum.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.TRUE">TRUE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.OVERRIDE_NON_DEFAULT">OVERRIDE_NON_DEFAULT</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> tables
        ///         with unique wal settings will be overridden when applying a
        ///         system level change.
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
        ///     <item>
        ///         <term><see
        ///         cref="Options.RESTORE_SYSTEM_SETTINGS">RESTORE_SYSTEM_SETTINGS</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> tables
        ///         with unique wal settings will be reverted to the current
        ///         global settings. Cannot be used in conjunction with any
        ///         other option.
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
        ///     <item>
        ///         <term><see cref="Options.PERSIST">PERSIST</see>:</term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see> and a
        ///         system-level change was requested, the system configuration
        ///         will be written to disk upon successful application of this
        ///         request. This will commit the changes from this request and
        ///         any additional in-memory modifications.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.TRUE">TRUE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public AlterWalRequest( IList<string> table_names,
                                IDictionary<string, string> options = null)
        {
            this.table_names = table_names ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AlterWalRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterWal(AlterWalRequest)">Kinetica.alterWal</see>.
    /// </summary>
    public class AlterWalResponse : KineticaData
    {
        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AlterWalResponse
} // end namespace kinetica
