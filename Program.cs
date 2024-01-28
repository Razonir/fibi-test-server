using fibi_test_server.Services;

namespace fibi_test_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<UserService>();

            builder.Services.AddControllers().AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);

            // Modify CORS policy to allow any origin without credentials
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AnyOrigin", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Use the modified CORS policy
            app.UseCors("AnyOrigin");

            app.MapControllers();

            app.Run();
        }
    }
}