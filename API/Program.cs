
using BLL.Services;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddTransient<SqlConnection>(sp =>
                new SqlConnection(builder.Configuration.GetConnectionString("default")));
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<UserRepo>();
            builder.Services.AddScoped<RunnerRepo>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(setup => {
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });

            });


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options => {
                        options.TokenValidationParameters = new TokenValidationParameters() {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(UserService.secretKey)),
                            ValidateLifetime = true,
                            ValidateIssuer = true,
                            ValidIssuer = "monapi.com",
                            ValidateAudience = false,
                            ClockSkew = TimeSpan.Zero,
                        };
                    }
                );

            builder.Services.AddAuthorization(options => {
                options.AddPolicy("AdminRequired", policy => policy.RequireRole("admin"));
                options.AddPolicy("OrganisatorRequired", policy => policy.RequireRole("admin", "organisator"));
                options.AddPolicy("UserRequired", policy => policy.RequireAuthenticatedUser());
            });

            builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
                o => o.AllowCredentials()
                      .WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //app.UseCors(o => o.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
                app.MapControllers().AllowAnonymous();
            else
                app.MapControllers();

            app.Run();
        }
    }
}
