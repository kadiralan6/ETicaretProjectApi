using ETicaretAPI.Services.Basket.Application.Services.CartService;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Basket.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly ICartService _cartService;

    public BasketController(ICartService cartService)
    {
        _cartService = cartService;
    }


}
