<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using WebThoiTrang.Models;


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

=======
>>>>>>> aeefa36c15e904858c8700698f6022722429480b
builder.Services.AddLogging();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
    //pattern: "{controller=Admin}/{action=IndexAdmin}/{id?}");
pattern: "{controller=Home}/{action=Bills}/{id?}");
//pattern: "{controller=Login}/{action=IndexLogin}/{id?}");
=======
    pattern: "{controller=Admin}/{action=IndexAdmin}/{id?}");

>>>>>>> aeefa36c15e904858c8700698f6022722429480b
app.Run();
