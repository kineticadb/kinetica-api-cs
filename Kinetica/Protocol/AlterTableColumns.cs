/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterTableColumns(AlterTableColumnsRequest)">Kinetica.alterTableColumns</see>.
    /// </summary>
    /// <remarks><para>Apply various modifications to columns in a table, view.
    /// The available modifications include the following:</para>
    /// <para>Create or delete an <a
    /// href="../../../concepts/indexes/#column-index" target="_top">index</a>
    /// on a particular column. This can speed up certain operations when using
    /// expressions containing equality or relational operators on indexed
    /// columns. This only applies to tables.</para>
    /// <para>Manage a table's columns--a column can be added, removed, or have
    /// its <a href="../../../concepts/types/" target="_top">type and
    /// properties</a> modified, including whether it is <a
    /// href="../../../concepts/dictionary_encoding/" target="_top">dictionary
    /// encoded</a> or not.</para></remarks>
    public class AlterTableColumnsRequest : KineticaData
    {
        /// <summary>Table on which the operation will be performed.</summary>
        /// <remarks><para>Must be an existing table or view, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</para></remarks>
        public string table_name { get; set; }

        /// <summary>List of alter table add/delete/change column requests -
        /// all for the same table.</summary>
        /// <remarks><para>Each request is a map that includes 'column_name',
        /// 'action' and the options specific for the action. Note that the
        /// same options as in alter table requests but in the same map as the
        /// column name and the action. For example:
        /// [{'column_name':'col_1','action':'change_column','rename_column':'col_2'},{'column_name':'col_1','action':'add_column',
        /// 'type':'int','default_value':'1'}]</para></remarks>
        public IList<IDictionary<string, string>> column_alterations { get; set; } = new List<IDictionary<string, string>>();

        /// <summary>Optional parameters.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AlterTableColumnsRequest object with default
        /// parameters.</summary>
        public AlterTableColumnsRequest() { }

        /// <summary>Constructs an AlterTableColumnsRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="table_name">Table on which the operation will be
        /// performed. Must be an existing table or view, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</param>
        /// <param name="column_alterations">List of alter table
        /// add/delete/change column requests - all for the same table. Each
        /// request is a map that includes 'column_name', 'action' and the
        /// options specific for the action. Note that the same options as in
        /// alter table requests but in the same map as the column name and the
        /// action. For example:
        /// [{'column_name':'col_1','action':'change_column','rename_column':'col_2'},{'column_name':'col_1','action':'add_column',
        /// 'type':'int','default_value':'1'}]</param>
        /// <param name="options">Optional parameters.</param>
        public AlterTableColumnsRequest( string table_name,
                                         IList<IDictionary<string, string>> column_alterations,
                                         IDictionary<string, string> options)
        {
            this.table_name = table_name ?? "";
            this.column_alterations = column_alterations ?? new List<IDictionary<string, string>>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AlterTableColumnsRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterTableColumns(AlterTableColumnsRequest)">Kinetica.alterTableColumns</see>.
    /// </summary>
    public class AlterTableColumnsResponse : KineticaData
    {
        /// <summary>Table on which the operation was performed.</summary>
        public string table_name { get; set; }

        /// <summary>return the type_id (when changing a table, a new type may
        /// be created)</summary>
        public string type_id { get; set; }

        /// <summary>return the type_definition  (when changing a table, a new
        /// type may be created)</summary>
        public string type_definition { get; set; }

        /// <summary>return the type properties  (when changing a table, a new
        /// type may be created)</summary>
        public IDictionary<string, IList<string>> properties { get; set; } = new Dictionary<string, IList<string>>();

        /// <summary>return the type label  (when changing a table, a new type
        /// may be created)</summary>
        public string label { get; set; }

        /// <summary>List of alter table add/delete/change column requests -
        /// all for the same table.</summary>
        /// <remarks><para>Each request is a map that includes 'column_name',
        /// 'action' and the options specific for the action. Note that the
        /// same options as in alter table requests but in the same map as the
        /// column name and the action. For example:
        /// [{'column_name':'col_1','action':'change_column','rename_column':'col_2'},{'column_name':'col_1','action':'add_column',
        /// 'type':'int','default_value':'1'}]</para></remarks>
        public IList<IDictionary<string, string>> column_alterations { get; set; } = new List<IDictionary<string, string>>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AlterTableColumnsResponse
} // end namespace kinetica
