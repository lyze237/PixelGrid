namespace PixelGrid.Shared.Models.Controller;

public record AuthRegisterRequest(string UserName, string Email, string Password);
public record AuthRegisterResponse;

public record AuthLoginRequest(string Email, string Password);
public record AuthLoginResponse(string Token);
