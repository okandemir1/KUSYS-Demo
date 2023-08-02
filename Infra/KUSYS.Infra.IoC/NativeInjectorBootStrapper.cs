using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KUSYS.Data;
using KUSYS.Data.Repository;
using KUSYS.Business.Interfaces;

namespace KUSYS.Infra.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static void AddContextInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<KUSYSDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));
        }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Services
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<ICourseService, CourseService>();

            // Infra - Data
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<KUSYSDbContext>();
        }
    }
}