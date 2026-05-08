using Microsoft.EntityFrameworkCore;
using TravelSphereMVC.Models;

namespace TravelSphereMVC.Data
{
    public static class DbSeeder
    {
        // Fixed ID for the system seeder admin. All seeded packages are owned
        // by this account so that freshly-registered admins start with empty dashboards.
        private const string SeederAdminId    = "seeder-admin-001";
        private const string SeederAdminEmail = "seed-admin@travelsphere.com";

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            // ── 1. Ensure the seeder admin account exists ─────────────
            if (!await context.Users.AnyAsync(u => u.Id == SeederAdminId))
            {
                var seederAdmin = new User
                {
                    Id               = SeederAdminId,
                    Name             = "TravelSphere System",
                    Mobile           = "0000000000",
                    Email            = SeederAdminEmail,
                    PasswordHash     = "SeederAdmin_" + Guid.NewGuid().ToString("N")[..8],
                    Role             = "Admin",
                    RegistrationDate = DateTime.UtcNow
                };
                context.Users.Add(seederAdmin);
                await context.SaveChangesAsync();
            }

            // ── 2. Seed demo packages (only if none exist) ────────────
            if (!await context.Packages.AnyAsync())
            {
                var packages = new List<Package>
                {
                    new Package
                    {
                        Id            = "ladakh",
                        AdminId       = SeederAdminId,
                        Name          = "Ladakh Adventure Tour",
                        Location      = "Ladakh, Jammu & Kashmir",
                        Duration      = "7 Days / 6 Nights",
                        Price         = 70000,
                        ImageUrl      = "/Images/ladakh.png",
                        ActivitiesList = "Trekking, River Rafting, Bike Ride, Camping",
                        Itinerary     = "Day 1: Arrival in Leh & Acclimatization\nDay 2: Sham Valley Sightseeing\nDay 3: Nubra Valley via Khardung La\nDay 4: Turtuk Village Exploration\nDay 5: Pangong Lake Visit\nDay 6: Return to Leh\nDay 7: Departure",
                        TotalSeats    = 12
                    },
                    new Package
                    {
                        Id            = "kashmir",
                        AdminId       = SeederAdminId,
                        Name          = "Kashmir Paradise Tour",
                        Location      = "Srinagar, Jammu & Kashmir",
                        Duration      = "6 Days / 5 Nights",
                        Price         = 28000,
                        ImageUrl      = "/Images/kashmir.png",
                        ActivitiesList = "Shikara Ride, Gondola, Shopping",
                        Itinerary     = "Day 1: Arrival Srinagar + Houseboat Stay\nDay 2: Gulmarg Visit\nDay 3: Sonmarg Trip\nDay 4: Pahalgam Tour\nDay 5: Local Market + Shikara Ride\nDay 6: Departure",
                        TotalSeats    = 15
                    },
                    new Package
                    {
                        Id            = "amberfort",
                        AdminId       = SeederAdminId,
                        Name          = "Amber Fort Royal Tour",
                        Location      = "Jaipur, Rajasthan",
                        Duration      = "3 Days / 2 Nights",
                        Price         = 9500,
                        ImageUrl      = "/Images/amber_fort.png",
                        ActivitiesList = "Heritage Walk, Shopping, Elephant Ride",
                        Itinerary     = "Day 1: Jaipur Arrival + Local Markets\nDay 2: Amber Fort + Nahargarh Fort\nDay 3: Hawa Mahal + Departure",
                        TotalSeats    = 20
                    },
                    new Package
                    {
                        Id            = "gateway",
                        AdminId       = SeederAdminId,
                        Name          = "Gateway of India City Escape",
                        Location      = "Mumbai, Maharashtra",
                        Duration      = "2 Days / 1 Night",
                        Price         = 6000,
                        ImageUrl      = "/Images/gateway_of_India.png",
                        ActivitiesList = "Ferry Ride, Marine Drive, Street Food",
                        Itinerary     = "Day 1: Gateway Visit + Ferry Ride\nDay 2: Marine Drive + Shopping + Departure",
                        TotalSeats    = 25
                    },
                    new Package
                    {
                        Id            = "varanasi",
                        AdminId       = SeederAdminId,
                        Name          = "Spiritual Varanasi Journey",
                        Location      = "Varanasi, Uttar Pradesh",
                        Duration      = "5 Days / 4 Nights",
                        Price         = 14000,
                        ImageUrl      = "/Images/varanasi.png",
                        ActivitiesList = "Ganga Aarti, Boat Ride, Temple Tour",
                        Itinerary     = "Day 1: Arrival + Evening Ganga Aarti\nDay 2: Temple Tour\nDay 3: Boat Ride + Local Markets\nDay 4: Sarnath Visit\nDay 5: Departure",
                        TotalSeats    = 18
                    },
                    new Package
                    {
                        Id            = "tajmahal",
                        AdminId       = SeederAdminId,
                        Name          = "Taj Mahal Heritage Tour",
                        Location      = "Agra, Uttar Pradesh",
                        Duration      = "2 Days / 1 Night",
                        Price         = 2500,
                        ImageUrl      = "/Images/taj_mahal.jpeg",
                        ActivitiesList = "Taj Mahal Visit, Agra Fort, Sunset View",
                        Itinerary     = "Day 1: Arrival + Taj Mahal Visit\nDay 2: Agra Fort + Local Shopping",
                        TotalSeats    = 30
                    }
                };

                await context.Packages.AddRangeAsync(packages);
                await context.SaveChangesAsync();
            }
            else
            {
                // ── 3. Back-fill AdminId on any legacy packages that have none ──
                var unowned = await context.Packages
                    .Where(p => p.AdminId == null)
                    .ToListAsync();

                if (unowned.Any())
                {
                    foreach (var pkg in unowned)
                        pkg.AdminId = SeederAdminId;

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
