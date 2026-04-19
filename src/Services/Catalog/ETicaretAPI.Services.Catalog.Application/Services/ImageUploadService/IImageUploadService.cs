using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ETicaretAPI.Services.Catalog.Application.Services.ImageUploadService;

public interface IImageUploadService
{
    Task<string> UploadImageAsync(IFormFile file);
}
