<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
=======
<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
>>>>>>> 12813279156ade9b8e34d2558f46c27b0b4e6d79
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebThoiTrang.Models;
using WebThoiTrang.Service;

=======
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Models;

>>>>>>> aeefa36c15e904858c8700698f6022722429480b
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DbContextShop>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("MyCS")); });
<<<<<<< HEAD
builder.Services.AddHttpContextAccessor();
=======

>>>>>>> aeefa36c15e904858c8700698f6022722429480b
builder.Services.AddControllersWithViews();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".AspNetCore.Antiforgery.YourApp";
});
<<<<<<< HEAD

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thiết lập thời gian timeout cho session
    options.Cookie.HttpOnly = true; // Chỉ cho phép cookie session được truy cập qua HTTP
    options.Cookie.IsEssential = true; // Đánh dấu cookie session là thiết yếu để tuân thủ GDPR
});

<<<<<<< HEAD
// Register CartService
builder.Services.AddScoped<CartService>();
=======
=======
>>>>>>> aeefa36c15e904858c8700698f6022722429480b
>>>>>>> 12813279156ade9b8e34d2558f46c27b0b4e6d79
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
<<<<<<< HEAD
app.UseSession();
=======

>>>>>>> aeefa36c15e904858c8700698f6022722429480b
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
<<<<<<< HEAD
    pattern: "{controller=Admin}/{action=ProductAdmin}/{id?}");
//pattern: "{controller=Admin}/{action=IndexAdmin}/{id?}");


=======
<<<<<<< HEAD
    //pattern: "{controller=Admin}/{action=IndexAdmin}/{id?}");
pattern: "{controller=Home}/{action=Bills}/{id?}");
//pattern: "{controller=Login}/{action=IndexLogin}/{id?}");
=======
    pattern: "{controller=Admin}/{action=IndexAdmin}/{id?}");

>>>>>>> aeefa36c15e904858c8700698f6022722429480b
>>>>>>> 12813279156ade9b8e34d2558f46c27b0b4e6d79
app.Run();
