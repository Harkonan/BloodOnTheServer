using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodOnTheWeb.Models
{
    public static class DbInitializer
    {
        public static void Initialize(SessionContext context)
        {
            context.Database.EnsureCreated();

            context.SaveChanges();
        }
    }
}
