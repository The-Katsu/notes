public class UserApi
{
    public void Register(WebApplication app)
    {
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
         return Results.Ok(token);
     });
    }
}