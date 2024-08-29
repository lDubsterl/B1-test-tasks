
using ExcelProcessing.Models;

namespace ExcelProcessing
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddTransient<ApplicationContext>();
			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
					builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					);
			});

			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			var app = builder.Build();

			app.UseCors();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			app.UseStaticFiles();

			app.Run();
		}
	}
}
