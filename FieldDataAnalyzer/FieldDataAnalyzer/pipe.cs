//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FieldDataAnalyzer
{
    using System;
    using System.Collections.Generic;
    
    public partial class pipe
    {
        public int pipe_id { get; set; }
        public string name { get; set; }
        public float length { get; set; }
        public float outer_d { get; set; }
        public float thickness { get; set; }
        public float inner_d { get; set; }
        public float roughness { get; set; }
        public int start_id { get; set; }
        public int end_id { get; set; }
        public float temper { get; set; }
    
        public virtual gather_points gather_points { get; set; }
        public virtual gather_points gather_points1 { get; set; }
    }
}
