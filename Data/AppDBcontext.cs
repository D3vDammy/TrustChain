
using Microsoft.EntityFrameworkCore;
using TrustChain.Models;              

namespace TrustChain.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Voter> Voters { get; set; }
    public DbSet<Election> Elections { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Vote> Votes { get; set; }
   

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Voter>()
            .HasIndex(v => v.NIN)
            .IsUnique();

        
        builder.Entity<Candidate>()
            .HasOne(c => c.Election)
            .WithMany(e => e.Candidates)
            .HasForeignKey(c => c.ElectionId);

        builder.Entity<Vote>()
            .HasOne(v => v.Voter)
            .WithMany()
            .HasForeignKey(v => v.VoterId);

        builder.Entity<Vote>()
            .HasOne(v => v.Candidate)
            .WithMany()
            .HasForeignKey(v => v.CandidateId);

        builder.Entity<Vote>()
            .HasOne(v => v.Election)
            .WithMany()
            .HasForeignKey(v => v.ElectionId);
    }
}