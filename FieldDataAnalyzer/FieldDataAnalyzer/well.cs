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
    
    public partial class well
    {
        public well()
        {
            this.wells_measurements = new HashSet<wells_measurements>();
        }
    
        public int well_id { get; set; }
        public Nullable<int> gather_point_id { get; set; }
        public string name { get; set; }
        public Nullable<float> a2 { get; set; }
        public Nullable<float> a3 { get; set; }
        public Nullable<float> b1 { get; set; }
        public Nullable<float> b2 { get; set; }
        public Nullable<float> b3 { get; set; }
        public Nullable<float> a1 { get; set; }
    
        public virtual gather_points gather_points { get; set; }
        public virtual ICollection<wells_measurements> wells_measurements { get; set; }
    }
}
