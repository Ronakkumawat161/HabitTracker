using HabitTracker.Infrastructure;
using HabitTracker.Repository;
using HabitTracker.Repository.Interface;
using HabitTracker.Service;
using HabitTracker.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<HabitTrackerDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("HTDatabaseConnection")));
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<HabitTrackerDbContext>()
            .AddDefaultUI();
            builder.Services.AddScoped<IHabitAnalyticsService, HabitAnalyticsService>();


            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IHabitRepository, HabitRepository>();
            builder.Services.AddScoped<IHabitService, HabitService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Habit}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
