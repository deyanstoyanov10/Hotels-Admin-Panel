namespace AdminPanelApp.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    public class RequisitonsConfig : IEntityTypeConfiguration<Requisitions>
    {
        public void Configure(EntityTypeBuilder<Requisitions> builder)
        {
            builder.HasMany(x => x.Products)
                .WithOne(x => x.Requisition)
                .HasForeignKey(x => x.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
