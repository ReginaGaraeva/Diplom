﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FieldDataAnalyzerDBEntities : DbContext
    {
        public FieldDataAnalyzerDBEntities()
            : base("name=FieldDataAnalyzerDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<final_gather_point_measurements> final_gather_point_measurements { get; set; }
        public DbSet<gather_points> gather_points { get; set; }
        public DbSet<gather_points_measurements> gather_points_measurements { get; set; }
        public DbSet<pipe> pipes { get; set; }
        public DbSet<well> wells { get; set; }
        public DbSet<wells_measurements> wells_measurements { get; set; }
    }
}
