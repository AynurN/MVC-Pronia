using Microsoft.EntityFrameworkCore;
using Pronia1.DAL;
using Pronia1.Services.Interfaces;

namespace Pronia1.Services.Implementations
{
    public class LayoutService :ILayoutService
    {
        private readonly AppDbContext context;

        public LayoutService(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<Dictionary<string,string>> GetSettings()
        {
          return await context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

        }
    }
}
