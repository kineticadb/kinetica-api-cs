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
    /// cref="Kinetica.evaluateModel(string,int,string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// </summary>
    public class EvaluateModelRequest : KineticaData
    {
        public string model_name { get; set; }
        public int replicas { get; set; }
        public string deployment_mode { get; set; }
        public string source_table { get; set; }
        public string destination_table { get; set; }
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an EvaluateModelRequest object with default
        /// parameters.</summary>
        public EvaluateModelRequest() { }

        /// <summary>Constructs an EvaluateModelRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="model_name"></param>
        /// <param name="replicas"></param>
        /// <param name="deployment_mode"></param>
        /// <param name="source_table"></param>
        /// <param name="destination_table"></param>
        /// <param name="options"></param>
        /// 
        public EvaluateModelRequest( string model_name,
                                     int replicas,
                                     string deployment_mode,
                                     string source_table,
                                     string destination_table,
                                     IDictionary<string, string> options = null)
        {
            this.model_name = model_name ?? "";
            this.replicas = replicas;
            this.deployment_mode = deployment_mode ?? "";
            this.source_table = source_table ?? "";
            this.destination_table = destination_table ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class EvaluateModelRequest
    /// @endcond



    /// @cond NO_DOCS
    /// <summary>A set of results returned by <see
    /// cref="Kinetica.evaluateModel(string,int,string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class EvaluateModelResponse : KineticaData
    {
        public string model_name { get; set; }
        public string destination_table { get; set; }
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class EvaluateModelResponse
    /// @endcond





}  // end namespace kinetica
