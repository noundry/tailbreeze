using Tailbreeze.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add Tailbreeze for Tailwind CSS integration
builder.Services.AddTailbreeze(options =>
{
    options.TailwindVersion = "latest"; // Use Tailwind CSS v4
    options.InputCssPath = "Styles/app.css";
    options.OutputCssPath = "css/app.css";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Use Tailbreeze middleware
app.UseTailbreeze();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
