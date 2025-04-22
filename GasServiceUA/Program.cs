using GasServiceUA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GasServiceUA.Models;
using MailKit;
using GasServiceUA.Services;
using GasServiceUA.UnitOfWork;
using GasServiceUA.Repositories;
using GasServiceUA.Helpers;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION_STRING");

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(CustomExceptionFilter));
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<EmailSettings>(options =>
{
    options.Server = Environment.GetEnvironmentVariable("SMTP_SERVER"); ;
    options.Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));
    options.SenderName = Environment.GetEnvironmentVariable("SMTP_SENDERNAME");
    options.SenderEmail = Environment.GetEnvironmentVariable("SMTP_SENDEREMAIL");
    options.UserName = Environment.GetEnvironmentVariable("SMTP_USERNAME");
    options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
});
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddScoped<IPayPalService>(x =>
    new PayPalService(
        Environment.GetEnvironmentVariable("PAYPAL_CLIENTID"),
        Environment.GetEnvironmentVariable("PAYPAL_CLIENTSECRET"),
        Environment.GetEnvironmentVariable("PAYPAL_MODE")
    )
);

builder.Services.AddTransient<ICurrencyConverterService, CurrencyConverterService>();
builder.Services.AddTransient<IDocumentCreatorService, PdfDocumentCreatorService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<User>), typeof(UserRepository));
builder.Services.AddScoped(typeof(IRepository<MeterReading>), typeof(MeterReadingRepository));
builder.Services.AddScoped(typeof(IRepository<Bill>), typeof(BillRepository));
builder.Services.AddScoped(typeof(IRepository<Payment>), typeof(PaymentRepository));
builder.Services.AddScoped(typeof(IRepository<Tariff>), typeof(TariffRepository));

var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => {
endpoints.MapControllerRoute(
    name: "UserAccount",
    pattern: "{area:exists}/{controller=UserAccount}/{action=Index}/{id?}");

endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
