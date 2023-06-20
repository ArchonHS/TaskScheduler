using Coravel;
using Microsoft.Extensions.Hosting.WindowsServices;
using TaskScheduler.Authorization;
using TaskScheduler.Authorization.Interfaces;
using TaskScheduler.Data;
using TaskScheduler.Data.Interfaces;
using TaskScheduler.Models;
using TaskScheduler.Services;
using TaskScheduler.Services.Interfaces;
using TaskScheduler.Swagger;

namespace TaskScheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
            {
                Args = args,
                ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default,
                ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName
            });
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.OperationFilter<ApiKeyHeaderFilter>();
            });
            builder.Services.AddAutoMapper(typeof(MapperProfile));
            builder.Services.AddScoped<IDapperService<TaskFromApiDTO>, DapperService<TaskFromApiDTO>>();
            builder.Services.AddScoped<ILoggerService, LoggerService>();
            builder.Services.AddScheduler();
            builder.Services.AddSingleton<ISchedulerService, SchedulerService>();
            builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
            builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

            builder.Host.UseWindowsService();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();
            app.Services.GetService<ISchedulerService>()
                .InitializeTasks();
            app.MapControllers();
            app.Run();
        }
    }
}