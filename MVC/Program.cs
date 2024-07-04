using MVC.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region SignalR Service

builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromMinutes(20);
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(40);
    options.HandshakeTimeout = TimeSpan.FromMinutes(5);
    options.MaximumParallelInvocationsPerClient = 10;
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
    options.StreamBufferCapacity = 50;
    options.EnableDetailedErrors = true;
}).AddAzureSignalR(/*azureconnectionString*/);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options => options.AddPolicy(MyAllowSpecificOrigins,
            builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed((host) => true)
                       .AllowCredentials();
            }));

#endregion SignalR Service End


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

app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapHub<ChatHub>("chat-hub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
