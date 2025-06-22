var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
	config.RegisterServicesFromAssembly(assembly);
	config.AddOpenBehavior(typeof(ValidationBehavior<,>));
	config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(opts =>
{
	opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
var app = builder.Build();

// Configure HTTP request pipeline
app.MapCarter();

app.UseExceptionHandler(options => { });

app.Run();