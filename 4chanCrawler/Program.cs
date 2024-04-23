using _4chanCrawler;
using _4chanCrawler.Helpers;
using _4chanCrawler.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = ConfigurationHelper.Get();

var serviceProvider = new ServiceCollection()
	.AddSingleton<CrawlerConfiguration>(configuration)
	.AddSingleton<CheckHelper>()
	.AddSingleton<App>()
	.AddSingleton<HttpClientHelper>()
	.AddSingleton<PrintHelper>()
	.AddSingleton<TimeHelper>()
	.BuildServiceProvider();

var app = serviceProvider.GetService<App>();
await app.Run();
