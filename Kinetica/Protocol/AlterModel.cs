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
    /// cref="Kinetica.alterModel(string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// </summary>
    public class AlterModelRequest : KineticaData
    {

        /// <summary>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterModelRequest.Action.CONTAINER">CONTAINER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterModelRequest.Action.REGISTRY">REGISTRY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterModelRequest.Action.REFRESH">REFRESH</see></term>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <see cref="action"
        /// />.</summary>
        public struct Action
        {
            public const string CONTAINER = "container";
            public const string REGISTRY = "registry";
            public const string REFRESH = "refresh";
        } // end struct Action

        public string model_name { get; set; }

        /// <summary>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterModelRequest.Action.CONTAINER">CONTAINER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterModelRequest.Action.REGISTRY">REGISTRY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterModelRequest.Action.REFRESH">REFRESH</see></term>
        ///     </item>
        /// </list></summary>
        public string action { get; set; }
        public string _value { get; set; }
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AlterModelRequest object with default
        /// parameters.</summary>
        public AlterModelRequest() { }

        /// <summary>Constructs an AlterModelRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="model_name"></param>
        /// <param name="action">
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterModelRequest.Action.CONTAINER">CONTAINER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterModelRequest.Action.REGISTRY">REGISTRY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterModelRequest.Action.REFRESH">REFRESH</see></term>
        ///     </item>
        /// </list></param>
        /// <param name="_value"></param>
        /// <param name="options"></param>
        /// 
        public AlterModelRequest( string model_name,
                                  string action,
                                  string _value,
                                  IDictionary<string, string> options = null)
        {
            this.model_name = model_name ?? "";
            this.action = action ?? "";
            this._value = _value ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AlterModelRequest
    /// @endcond



    /// @cond NO_DOCS
    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterModel(string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class AlterModelResponse : KineticaData
    {
        public string model_name { get; set; }
        public string action { get; set; }
        public string _value { get; set; }
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AlterModelResponse
    /// @endcond





}  // end namespace kinetica
