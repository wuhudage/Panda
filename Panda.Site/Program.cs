using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Panda.Admin;
using Panda.App;
using Panda.Site.Configs;
using Panda.Entity;
using Panda.Site.Admin;
using Panda.Tools;
using Panda.Tools.QueueTask;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Configuration.AddIniFile($"Site.{env.EnvironmentName}.ini", false, true);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",new OpenApiInfo()
    {
        Title = "后台api文档",
        Version = "v1",
        Description = ""
    });
    var list = new string[] { "Panda.Site.xml", "Panda.Site.Admin.xml", "Panda.Admin.xml" };
    foreach (var item in list)
    {
        var file = Path.Combine(AppContext.BaseDirectory, item);
        var path = Path.Combine(AppContext.BaseDirectory, file);
        c.IncludeXmlComments(path,true);
        c.OrderActionsBy(o=>o.RelativePath);
    }
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy  =>
        {
            policy.WithOrigins("https://editor.swagger.io")
                .AllowAnyHeader().AllowAnyMethod();
        });
});


// Add services to the container.
builder.Services.AddRazorPages().AddMvcOptions(opt => { opt.Filters.Add<StatisticFilter>(); });


var services = builder.Services;

services.AddHttpClient();
services.AddDbContext<PandaContext>(
    opt =>
    {
        var db = builder.Configuration.GetConnectionString("MYSQL");
        opt.UseLazyLoadingProxies()
            .UseMySql(db, ServerVersion.AutoDetect(db))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
);

services.AddConfig(builder.Configuration);

services.AddQueueTask();

services.AddTools();
services.AddAutoInject(opt =>
{
    opt.Options.Add(new AutoInjectOptionItem
    {
        EndWdith = "Service"
    });
    opt.Options.Add(new AutoInjectOptionItem
    {
        EndWdith = "Repository",
        InjectSelf = true
    });
});
//Site.Development.ini

builder.AddAdmin<PandaContext>();
services.AddSiteAdmin();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
   
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();


app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);


app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.UseStatusCodePagesWithReExecute("/{0}.html");
app.MapRazorPages();

app.Run();