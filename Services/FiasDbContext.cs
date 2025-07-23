using Microsoft.EntityFrameworkCore;
using FiasApi.Models;

namespace FiasApi.Services;

public class FiasDbContext : DbContext
{
    public FiasDbContext(DbContextOptions<FiasDbContext> options) : base(options) {}

    public DbSet<AddrObj> AddrObjs { get; set; }
}
