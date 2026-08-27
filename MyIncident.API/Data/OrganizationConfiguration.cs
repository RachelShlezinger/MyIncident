using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyIncident.API.Models;

namespace MyIncident.API.Data;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(o => o.HandlerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(o => o.Name).IsUnique();

        builder.HasMany(o => o.Requests)
            .WithOne(r => r.Organization)
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
