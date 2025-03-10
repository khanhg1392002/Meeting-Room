using MeetingRoomAPI.Repositories;
using MeetingRoomAPI.Services;
using MeetingRoomAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Giữ nguyên tên thuộc tính
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký DatabaseContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentNullException(nameof(connectionString), "Connection string 'DefaultConnection' not found in appsettings.json.");
}
builder.Services.AddScoped<DatabaseContext>(provider => new DatabaseContext(connectionString));

// Đăng ký Repositories
builder.Services.AddScoped<BranchRepository>();
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<UserRepository>();

// Đăng ký Services
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IUserService, UserService>();

// Thêm CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Sử dụng CORS
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();