#region File Attributes

// AdminWeb  Project: VirtueBuild.Data.Context
// File:  BrandMap.cs
// Created By: Slava Khristich 
// Created On: 2013.06.04 
// http://tateeda.com

#endregion

#region

#endregion

namespace VirtueBuild.Data.Context.Mapping {
    #region

    using System.Data.Entity.ModelConfiguration;

    using Domain.Lookups;

    #endregion

    public class BrandMap : EntityTypeConfiguration<Brand> {

        public BrandMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name).IsRequired().HasMaxLength(255);

            // Table & Column Mappings
            ToTable("Brand", "Lookup");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.SortOrder).HasColumnName("SortOrder");
        }

    }
}