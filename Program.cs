using System.Text;
using Resend;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TrustChain.Data;
using TrustChain.Hubs;
using TrustChain.Middleware;
using TrustChain.Services;
using TrustChain.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── DATABASE
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TrustChainDb"));

// ── SIGNALR 
builder.Services.AddSignalR();

// ── SERVICES
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IElectionService, ElectionService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// ── JWT AUTH 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var token = ctx.Request.Query["access_token"];
                var path = ctx.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/hubs"))
                    ctx.Token = token;
                return Task.CompletedTask;
            }
        };
    });

// ── CORS 
builder.Services.AddCors(opt => opt.AddPolicy("AllowFrontend", p =>
    p.WithOrigins(
        "http://localhost:3000",
        "http://localhost:5074 ",
        "http://localhost:4200"
    )
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
));

// ── CONTROLLERS 
builder.Services.AddControllers();


// ── RESEND EMAIL SERVICE
builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = builder.Configuration["Resend:ApiToken"]!;
});
builder.Services.AddTransient<IResend, ResendClient>();

// ── SWAGGER
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
// ── BUILD APP
var app = builder.Build();

// ── SWAGGER UI 
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrustChain Voting API v1");
    
});

// ── MIDDLEWARE 
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<VoteHub>("/hubs/vote");

app.Run();
