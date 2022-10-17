using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Simple_Stocks.Services;

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


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
