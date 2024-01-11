using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

var options=new LaunchOptions{Headless=true,Args=new string[]{"--no-sandbox"}};
Console.WriteLine("Downloading chromium");
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();

app.MapGet("/output",async () =>
{
   Console.WriteLine("This is from API output. Navigating google");
   using (var browser = await Puppeteer.LaunchAsync(options))
   using (var page = await browser.NewPageAsync())
   {
       await page.GoToAsync("http://www.google.com");
       Console.WriteLine("Generating PDF");
       await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "google.pdf"));

       Console.WriteLine("Export completed");
       if (!args.Any(arg => arg == "auto-exit"))
       {
          Console.ReadLine();
       }
   }
   return Results.NoContent();
}		
);

app.Run();
