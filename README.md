# Notes API 
---
# Welcome to my pet project
The idea is to do API with authentication using C# and MongoDB.  
Stack: ASP.NET Core 6 minimal API, MongoDB, JWT token.  

---
### Create project
To create project write console command
```Powershell
dotnet new web -n ProjectName
```
Add references to work with MongoDb and JWT token with NuGet or AddPackages console command.  
Now .csproj contains:
```XML  
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- JWT  -->
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="6.0.1" />
    <PackageReference Include="Microsoft.IdentityModel.Tokens" Version="6.15.1" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="6.15.1" />
    <!-- MongoDB -->
    <PackageReference Include="MongoDB.Bson" Version="2.14.1" />
    <PackageReference Include="MongoDB.Driver" Version="2.14.1" />
  </ItemGroup>

</Project>
```
### Connect to database
Add connection string to appsettings.json
```JSON
"NotesDatabase": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "NotesDb",
    "CollectionName": "NotesList"
  }
```
Add connection in Program.cs
```csharp
builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("NotesDatabase"));
```
Add DbSetting class
```csharp
public class DbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}
```
Now create class that will set connection and work with database
```csharp
public class NotesService
{
    private readonly IMongoCollection<Note> _collection;

    public NotesService(IOptions<DbSettings> dbSettings)
    {
        var mongoClient = new MongoClient(
                dbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dbSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<Note>(
            dbSettings.Value.CollectionName);
    }

    public async Task CreateAsync(Note note) =>
        await _collection.InsertOneAsync(note);

    public async Task UpdateAsync(Note note) =>
        await _collection.ReplaceOneAsync(x => x.Id == note.Id, note);

    public async Task<List<Note>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<Note?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task RemoveAsync(string id) => 
        await _collection.DeleteOneAsync(x => x.Id == id);
}
```
### Add JWT
Create simple JWT authentication with one user.  
Create user record class 
```csharp
public record UserDto (string UserName, string Password);

public record User
{
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
```

Create in code list of users and getter 
```csharp
public class UserService
{
    private List<UserDto> _users => new()
    {
        new UserDto("Vladimir", "123")
    };

    public UserDto GetUser(User user) =>
      _users.FirstOrDefault(u => 
            string.Equals(u.UserName, user.UserName) &&
            string.Equals(u.Password, user.Password)) ??
            throw new Exception();  
}
```

Create service that will return JWT token by our user  
Add secret to appsettings.json
```JSON
"Jwt": {
    "Key": "Katsu1234567890z",
    "Issuer": "Katsu",
    "Audience": "Katsu"
  }
```
Then create service class
```csharp
public class TokenService
{
    private TimeSpan ExpiryDuration = new TimeSpan(1, 0, 0);

    public string BuildToken(string key, string issuer, UserDto user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
        };
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, 
        SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
            expires: DateTime.Now.Add(ExpiryDuration),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
```
Register authentication in Program.cs
```csharp
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


app.UseAuthentication();
app.UseAuthorization();
```
### API part
Now create requests  
Login request
```csharp
app.MapGet("/login", [AllowAnonymous] (HttpContext context,
            TokenService tokenService, UserService userService) => {
         User user = new()
         {
             UserName = context.Request.Query["username"],
             Password = context.Request.Query["password"]
         };
         var userDto = userService.GetUser(user);
         if (userDto == null) return Results.Unauthorized();
         var token = tokenService.BuildToken(app.Configuration["Jwt:Key"],
            app.Configuration["Jwt:Issuer"],
            userDto);
         return Results.Ok(token);});
```
Notes requests
```csharp
app.MapGet("/", [Authorize] async (NotesService service) =>
            await service.GetAsync());

app.MapGet("/{id:length(24)}", [Authorize] async (string id, NotesService service) =>
    await service.GetAsync(id) is Note item
    ? Results.Ok(item)
    : Results.NotFound());

app.MapPost("/", [Authorize] async ([FromBody] Note item, NotesService service) => {
    if (item.Id == null)
        await service.CreateAsync(item);
    else await service.UpdateAsync(item);
    return Results.Created($"/{item.Id}", item);
});

app.MapDelete("/{id:length(24)}", [Authorize] async (string id, NotesService service) => {
    await service.RemoveAsync(id);
    return Results.NoContent();
});
```
Don't forget to register our services in Program.cs
```csharp
builder.Services.AddSingleton<NotesService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<TokenService>();
```
---
## Try some requests  
Type in console
```Powershell
dotnet run

Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7093
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5178
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
```
Using Postman or Thunder Client  
Authenticate  
![Login](https://clck.ru/dBoZp)  
Place token here  
![Token](https://clck.ru/dBp2D)  
Add note  
![AddNote](https://clck.ru/dBpLB)  
Add more notes and get list of them  
![List](https://clck.ru/dBpe8)  
Get one  
![Get](https://clck.ru/dBps6)  
Delete one  
![Delete](https://clck.ru/dBq6z)  
![AfterDelete](https://clck.ru/dBqSU)  