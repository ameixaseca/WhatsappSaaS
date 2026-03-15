using Microsoft.EntityFrameworkCore;
using WhatsappSaaS.Domain.Entities;
using WhatsappSaaS.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace WhatsappSaaS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ChatbotFlow> ChatbotFlows { get; set; }
    public DbSet<FlowStep> FlowSteps { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<QuickResponse> QuickResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Multi-tenancy filter
        modelBuilder.Entity<User>().HasQueryFilter(e => e.CompanyId == _tenantService.CompanyId);
        modelBuilder.Entity<Contact>().HasQueryFilter(e => e.CompanyId == _tenantService.CompanyId);
        modelBuilder.Entity<Conversation>().HasQueryFilter(e => e.CompanyId == _tenantService.CompanyId);
        modelBuilder.Entity<Message>().HasQueryFilter(e => e.CompanyId == _tenantService.CompanyId);
        modelBuilder.Entity<Tag>().HasQueryFilter(e => e.CompanyId == _tenantService.CompanyId);
        modelBuilder.Entity<ChatbotFlow>().HasQueryFilter(e => e.CompanyId == _tenantService.CompanyId);
        modelBuilder.Entity<FlowStep>().HasQueryFilter(e => e.CompanyId == _tenantService.CompanyId);
        modelBuilder.Entity<Appointment>().HasQueryFilter(e => e.CompanyId == _tenantService.CompanyId);
        modelBuilder.Entity<QuickResponse>().HasQueryFilter(e => e.CompanyId == _tenantService.CompanyId);

        // Configure relationships
        modelBuilder.Entity<Company>()
            .HasOne(c => c.Plan)
            .WithMany(p => p.Companies)
            .HasForeignKey(c => c.PlanId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CompanyId);

        modelBuilder.Entity<Contact>()
            .HasOne(c => c.Company)
            .WithMany(co => co.Contacts)
            .HasForeignKey(c => c.CompanyId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<MultiTenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CompanyId = _tenantService.CompanyId;
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
