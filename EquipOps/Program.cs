<<<<<<< Updated upstream
using EquipOps.API;
=======
﻿using EquipOps.API;
using EquipOps.Common.Configuration;
using Microsoft.AspNetCore.Builder;
>>>>>>> Stashed changes

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();

builder.Services.WithRegisterServices();

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigin", policy =>
	{
		policy.WithOrigins("http://localhost:5174") // ✅ your frontend dev port
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});
var app = builder.Build();
app.UseCors("AllowAllOrigin");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
                                                                                                                                                                                             
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
