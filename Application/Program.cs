using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infra.Interfaces;
using Infra.Repositories;
using Application.Service.Application;
using Application.Service.Interfaces;
using Domain.Service.Interfaces;
using Domain.Service.Services.ServiceApi;
using Domain.Service.Services.ServiceJwt;
using Domain.Service.Services.ServiceApiExternal;
using Infra;
using Domain.Service.Service.ServiceAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Application.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
        builder.WithOrigins("*")
               .AllowAnyMethod()
               .AllowAnyHeader());
});

var secret = configuration["Jwt:Key"];
var key = Encoding.ASCII.GetBytes(secret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = configuration["Jwt:Authority"];
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Chatbot Dotnet", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by space and JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddAntiforgery();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();

builder.Services.AddHttpClient<JwtTokenServiceAPI>(client =>
{
    client.BaseAddress = new Uri("http://192.168.10.40:92");
    client.DefaultRequestHeaders.Add("ApplicationKey", "MzE0NzFlNWYyMjdhLTQzYWItOTRjMi0wZGZlMDdmZDhlNGE=");
});


builder.Services.AddScoped<IUserApplication, UserApplication>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IQuestionApplication, QuestionApplication>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISessionApplication, SessionApplication>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IConversationApplication, ConversationApplication>();
builder.Services.AddScoped<IJwtTokenServiceAPI, JwtTokenServiceAPI>();
builder.Services.AddScoped<IJwtTokenApplicationAPI, JwtTokenApplicationAPI>();
builder.Services.AddScoped<RequestConversationService>();
builder.Services.AddScoped<HashPasswordService>();
builder.Services.AddScoped<RequestSessionService>();
builder.Services.AddScoped<ConnectClientService>();
builder.Services.AddScoped<ContextDatabase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chatbot API v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await InitializeApplicationAsync(app);

app.Run();

async Task InitializeApplicationAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var jwtTokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenServiceAPI>();
        await jwtTokenService.InitializeAsync();
    }
}
