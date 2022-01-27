/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{
    /// @cond NO_DOCS
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterGraph(string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// </summary>
    public class AlterGraphRequest : KineticaData
    {

        /// <summary>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.ADD_TABLE_MONITOR">ADD_TABLE_MONITOR</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.RESET_CLIENT">RESET_CLIENT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.RESET_SERVER">RESET_SERVER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.CANCEL_TASK">CANCEL_TASK</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.ALTER_LOGGER">ALTER_LOGGER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.DELETE_ALL">DELETE_ALL</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.STATUS">STATUS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.COLLECT_GRAPHS">COLLECT_GRAPHS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.RESTORE_GRAPHS">RESTORE_GRAPHS</see></term>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <see cref="action"
        /// />.</summary>
        public struct Action
        {
            public const string ADD_TABLE_MONITOR = "add_table_monitor";
            public const string RESET_CLIENT = "reset_client";
            public const string RESET_SERVER = "reset_server";
            public const string CANCEL_TASK = "cancel_task";
            public const string ALTER_LOGGER = "alter_logger";
            public const string DELETE_ALL = "delete_all";
            public const string STATUS = "status";
            public const string COLLECT_GRAPHS = "collect_graphs";
            public const string RESTORE_GRAPHS = "restore_graphs";
        } // end struct Action


        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Options.SERVER_ID">SERVER_ID</see>:</term>
        ///         <description></description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {
            public const string SERVER_ID = "server_id";
        } // end struct Options

        public string graph_name { get; set; }

        /// <summary>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.ADD_TABLE_MONITOR">ADD_TABLE_MONITOR</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.RESET_CLIENT">RESET_CLIENT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.RESET_SERVER">RESET_SERVER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.CANCEL_TASK">CANCEL_TASK</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.ALTER_LOGGER">ALTER_LOGGER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.DELETE_ALL">DELETE_ALL</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.STATUS">STATUS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.COLLECT_GRAPHS">COLLECT_GRAPHS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.RESTORE_GRAPHS">RESTORE_GRAPHS</see></term>
        ///     </item>
        /// </list></summary>
        public string action { get; set; }
        public string action_arg { get; set; }

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Options.SERVER_ID">SERVER_ID</see>:</term>
        ///         <description></description>
        ///     </item>
        /// </list>
        /// </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AlterGraphRequest object with default
        /// parameters.</summary>
        public AlterGraphRequest() { }

        /// <summary>Constructs an AlterGraphRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="graph_name"></param>
        /// <param name="action">
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.ADD_TABLE_MONITOR">ADD_TABLE_MONITOR</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.RESET_CLIENT">RESET_CLIENT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.RESET_SERVER">RESET_SERVER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.CANCEL_TASK">CANCEL_TASK</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.ALTER_LOGGER">ALTER_LOGGER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.DELETE_ALL">DELETE_ALL</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.STATUS">STATUS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.COLLECT_GRAPHS">COLLECT_GRAPHS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Action.RESTORE_GRAPHS">RESTORE_GRAPHS</see></term>
        ///     </item>
        /// </list></param>
        /// <param name="action_arg"></param>
        /// <param name="options">
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterGraphRequest.Options.SERVER_ID">SERVER_ID</see>:</term>
        ///         <description></description>
        ///     </item>
        /// </list>
        /// </param>
        /// 
        public AlterGraphRequest( string graph_name,
                                  string action,
                                  string action_arg,
                                  IDictionary<string, string> options = null)
        {
            this.graph_name = graph_name ?? "";
            this.action = action ?? "";
            this.action_arg = action_arg ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AlterGraphRequest
    /// @endcond



    /// @cond NO_DOCS
    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterGraph(string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class AlterGraphResponse : KineticaData
    {
        public string action { get; set; }
        public string action_arg { get; set; }
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AlterGraphResponse
    /// @endcond





}  // end namespace kinetica