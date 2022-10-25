using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Simple_Stocks.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("StocksCS");
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<StocksDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddMvc()
                    .AddNewtonsoftJson(o =>
                    {
                        o.SerializerSettings.ReferenceLoopHandling =
                            Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                        o.SerializerSettings.ContractResolver =
                            new CamelCasePropertyNamesContractResolver();
                    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("JWT:Secret").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IRightRepo, RightRepo>();
builder.Services.AddScoped<IRoleRepo, RoleRepo>();
builder.Services.AddScoped<IRoleRightRepo, RoleRightRepo>();
builder.Services.AddScoped<ITagRepo, TagRepo>();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<IPostTagRepo, PostTagRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<ILikedCommentRepo, LikedCommentRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<IUserBlockRepo, UserBlockRepo>();
builder.Services.AddScoped<IUserFollowRepo, UserFollowRepo>();
builder.Services.AddScoped<ILikedPostRepo, LikedPostRepo>();
builder.Services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
