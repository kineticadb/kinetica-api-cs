/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createProc(string,string,IDictionary{string, byte[]},string,IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Creates an instance (proc) of the
    /// <a href="../../../concepts/udf/" target="_top">user-defined
    /// functions</a> (UDF) specified by the
    /// given command, options, and files, and makes it available for
    /// execution.</summary>
    public class CreateProcRequest : KineticaData
    {

        /// <summary>The execution mode of the proc.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateProcRequest.ExecutionMode.DISTRIBUTED">DISTRIBUTED</see>:</term>
        ///         <description>Input table data will be divided into data
        /// segments that are distributed across all nodes in the cluster, and
        /// the proc
        /// command will be invoked once per data segment in parallel. Output
        /// table data
        /// from each invocation will be saved to the same node as the
        /// corresponding input
        /// data.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateProcRequest.ExecutionMode.NONDISTRIBUTED">NONDISTRIBUTED</see>:</term>
        ///         <description>The proc command will be invoked only once per
        /// execution, and will not have direct access to any tables named as
        /// input or
        /// output table parameters in the call to /execute/proc.  It will,
        /// however, be able to access the database using native API
        /// calls.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateProcRequest.ExecutionMode.DISTRIBUTED">DISTRIBUTED</see>.
        /// A set of string constants for the parameter <see
        /// cref="execution_mode" />.</summary>
        public struct ExecutionMode
        {

            /// <summary>Input table data will be divided into data
            /// segments that are distributed across all nodes in the cluster,
            /// and the proc
            /// command will be invoked once per data segment in parallel.
            /// Output table data
            /// from each invocation will be saved to the same node as the
            /// corresponding input
            /// data.</summary>
            public const string DISTRIBUTED = "distributed";

            /// <summary>The proc command will be invoked only once per
            /// execution, and will not have direct access to any tables named
            /// as input or
            /// output table parameters in the call to <see
            /// cref="Kinetica.executeProc(string,IDictionary{string, string},IDictionary{string, byte[]},IList{string},IDictionary{string, IList{string}},IList{string},IDictionary{string, string})"
            /// />.  It will,
            /// however, be able to access the database using native API
            /// calls.</summary>
            public const string NONDISTRIBUTED = "nondistributed";
        } // end struct ExecutionMode


        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateProcRequest.Options.MAX_CONCURRENCY_PER_NODE">MAX_CONCURRENCY_PER_NODE</see>:</term>
        ///         <description>The maximum number of concurrent instances of
        /// the proc that will be executed per node. 0 allows unlimited
        /// concurrency.  The default value is '0'.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>The maximum number of concurrent instances of the proc
            /// that will be executed per node. 0 allows unlimited concurrency.
            /// The default value is '0'.</summary>
            public const string MAX_CONCURRENCY_PER_NODE = "max_concurrency_per_node";
        } // end struct Options


        /// <summary>Name of the proc to be created. Must not be the name of a
        /// currently existing proc.  </summary>
        public string proc_name { get; set; }

        /// <summary>The execution mode of the proc.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateProcRequest.ExecutionMode.DISTRIBUTED">DISTRIBUTED</see>:</term>
        ///         <description>Input table data will be divided into data
        /// segments that are distributed across all nodes in the cluster, and
        /// the proc
        /// command will be invoked once per data segment in parallel. Output
        /// table data
        /// from each invocation will be saved to the same node as the
        /// corresponding input
        /// data.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateProcRequest.ExecutionMode.NONDISTRIBUTED">NONDISTRIBUTED</see>:</term>
        ///         <description>The proc command will be invoked only once per
        /// execution, and will not have direct access to any tables named as
        /// input or
        /// output table parameters in the call to /execute/proc.  It will,
        /// however, be able to access the database using native API
        /// calls.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateProcRequest.ExecutionMode.DISTRIBUTED">DISTRIBUTED</see>.
        /// </summary>
        public string execution_mode { get; set; } = ExecutionMode.DISTRIBUTED;

        /// <summary>A map of the files that make up the proc. The keys of the
        /// map are file names, and the values are the binary contents of the
        /// files. The
        /// file names may include subdirectory names (e.g. 'subdir/file') but
        /// must not
        /// resolve to a directory above the root for the proc.  The default
        /// value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, byte[]> files { get; set; } = new Dictionary<string, byte[]>();

        /// <summary>The command (excluding arguments) that will be invoked
        /// when
        /// the proc is executed. It will be invoked from the directory
        /// containing the proc
        /// <paramref cref="CreateProcRequest.files" /> and may be any command
        /// that can be resolved from that directory.
        /// It need not refer to a file actually in that directory; for
        /// example, it could be
        /// 'java' if the proc is a Java application; however, any necessary
        /// external
        /// programs must be preinstalled on every database node. If the
        /// command refers to a
        /// file in that directory, it must be preceded with './' as per Linux
        /// convention.
        /// If not specified, and exactly one file is provided in <paramref
        /// cref="CreateProcRequest.files" />, that file
        /// will be invoked.  The default value is ''.</summary>
        public string command { get; set; } = "";

        /// <summary>An array of command-line arguments that will be passed to
        /// <paramref cref="CreateProcRequest.command" /> when the proc is
        /// executed.  The default value is an empty {@link List}.</summary>
        public IList<string> args { get; set; } = new List<string>();

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateProcRequest.Options.MAX_CONCURRENCY_PER_NODE">MAX_CONCURRENCY_PER_NODE</see>:</term>
        ///         <description>The maximum number of concurrent instances of
        /// the proc that will be executed per node. 0 allows unlimited
        /// concurrency.  The default value is '0'.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateProcRequest object with default
        /// parameters.</summary>
        public CreateProcRequest() { }

        /// <summary>Constructs a CreateProcRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="proc_name">Name of the proc to be created. Must not be
        /// the name of a currently existing proc.  </param>
        /// <param name="execution_mode">The execution mode of the proc.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateProcRequest.ExecutionMode.DISTRIBUTED">DISTRIBUTED</see>:</term>
        ///         <description>Input table data will be divided into data
        /// segments that are distributed across all nodes in the cluster, and
        /// the proc
        /// command will be invoked once per data segment in parallel. Output
        /// table data
        /// from each invocation will be saved to the same node as the
        /// corresponding input
        /// data.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateProcRequest.ExecutionMode.NONDISTRIBUTED">NONDISTRIBUTED</see>:</term>
        ///         <description>The proc command will be invoked only once per
        /// execution, and will not have direct access to any tables named as
        /// input or
        /// output table parameters in the call to /execute/proc.  It will,
        /// however, be able to access the database using native API
        /// calls.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateProcRequest.ExecutionMode.DISTRIBUTED">DISTRIBUTED</see>.
        /// </param>
        /// <param name="files">A map of the files that make up the proc. The
        /// keys of the
        /// map are file names, and the values are the binary contents of the
        /// files. The
        /// file names may include subdirectory names (e.g. 'subdir/file') but
        /// must not
        /// resolve to a directory above the root for the proc.  The default
        /// value is an empty {@link Dictionary}.</param>
        /// <param name="command">The command (excluding arguments) that will
        /// be invoked when
        /// the proc is executed. It will be invoked from the directory
        /// containing the proc
        /// <paramref cref="CreateProcRequest.files" /> and may be any command
        /// that can be resolved from that directory.
        /// It need not refer to a file actually in that directory; for
        /// example, it could be
        /// 'java' if the proc is a Java application; however, any necessary
        /// external
        /// programs must be preinstalled on every database node. If the
        /// command refers to a
        /// file in that directory, it must be preceded with './' as per Linux
        /// convention.
        /// If not specified, and exactly one file is provided in <paramref
        /// cref="CreateProcRequest.files" />, that file
        /// will be invoked.  The default value is ''.</param>
        /// <param name="args">An array of command-line arguments that will be
        /// passed to <paramref cref="CreateProcRequest.command" /> when the
        /// proc is executed.  The default value is an empty {@link
        /// List}.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateProcRequest.Options.MAX_CONCURRENCY_PER_NODE">MAX_CONCURRENCY_PER_NODE</see>:</term>
        ///         <description>The maximum number of concurrent instances of
        /// the proc that will be executed per node. 0 allows unlimited
        /// concurrency.  The default value is '0'.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public CreateProcRequest( string proc_name,
                                  string execution_mode = null,
                                  IDictionary<string, byte[]> files = null,
                                  string command = null,
                                  IList<string> args = null,
                                  IDictionary<string, string> options = null)
        {
            this.proc_name = proc_name ?? "";
            this.execution_mode = execution_mode ?? ExecutionMode.DISTRIBUTED;
            this.files = files ?? new Dictionary<string, byte[]>();
            this.command = command ?? "";
            this.args = args ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateProcRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createProc(string,string,IDictionary{string, byte[]},string,IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class CreateProcResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="CreateProcRequest.proc_name" />.
        /// </summary>
        public string proc_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateProcResponse




}  // end namespace kinetica
