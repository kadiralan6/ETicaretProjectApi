using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Application.DTOs.CatalogDtos;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Application.Services.ImageUploadService;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductImageDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Services.ProductImageService;

public class ProductImageManager : IProductImageService
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IImageUploadService _imageUploadService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    private static string ProductCacheKey(int productId) => $"catalog:product:{productId}";

    public ProductImageManager(
        IProductImageRepository productImageRepository,
        IImageUploadService imageUploadService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _productImageRepository = productImageRepository;
        _imageUploadService = imageUploadService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<List<GetProductImageDto>>> GetImagesByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var images = await _productImageRepository.GetAllWithAsNoTrackingAsync(
            x => x.ProductId == productId, cancellationToken: cancellationToken);

        var result = _mapper.Map<List<GetProductImageDto>>(images.ToList());
        return ApiResponse<List<GetProductImageDto>>.Success(result);
    }

    public async Task<ApiResponse<GetProductImageDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetWithAsNoTrackingAsync(
            x => x.Id == id, cancellationToken: cancellationToken);

        if (image is null)
            throw new NotFoundException(nameof(ProductImage), id);

        var result = _mapper.Map<GetProductImageDto>(image);
        return ApiResponse<GetProductImageDto>.Success(result);
    }

    public async Task<ApiResponse<List<GetProductImageDto>>> CreateAsync(CreateProductImageDto dto, CancellationToken cancellationToken = default)
    {
        var createdImages = new List<ProductImage>();

        if (dto.Files != null && dto.Files.Count > 0)
        {
            foreach (var file in dto.Files)
            {
                if (file != null && file.Length > 0)
                {
                    var uploadedUrl = await _imageUploadService.UploadImageAsync(file);
                    var image = new ProductImage
                    {
                        ProductId = dto.ProductId,
                        IsCover = dto.IsCover,
                        Url = uploadedUrl,
                        AltText = dto.AltText
                    };
                    createdImages.Add(image);
                }
            }
        }
        else
        {
            var image = _mapper.Map<ProductImage>(dto);
            createdImages.Add(image);
        }

        if (dto.IsCover)
        {
            var otherCovers = await _productImageRepository.GetAllAsync(
                x => x.ProductId == dto.ProductId && x.IsCover, cancellationToken: cancellationToken);
            foreach (var cover in otherCovers)
            {
                cover.IsCover = false;
                await _productImageRepository.UpdateAsync(cover, cancellationToken);
            }
        }

        foreach (var image in createdImages)
        {
            await _productImageRepository.AddAsync(image, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync(ProductCacheKey(dto.ProductId), cancellationToken);

        var result = _mapper.Map<List<GetProductImageDto>>(createdImages);
        return ApiResponse<List<GetProductImageDto>>.Success(result);
    }

    public async Task<ApiResponse<List<GetProductImageDto>>> UpdateAsync(UpdateProductImageDto dto, CancellationToken cancellationToken = default)
    {
        var existingImage = await _productImageRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        var savedImages = new List<ProductImage>();

        if (existingImage is null)
        {
            if (dto.Files != null && dto.Files.Count > 0)
            {
                foreach (var file in dto.Files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var uploadedUrl = await _imageUploadService.UploadImageAsync(file);
                        savedImages.Add(new ProductImage
                        {
                            ProductId = dto.ProductId,
                            Url = uploadedUrl
                        });
                    }
                }
            }
            else
            {
                savedImages.Add(_mapper.Map<ProductImage>(dto));
            }
        }
        else
        {
            _mapper.Map(dto, existingImage);
            if (dto.Files != null && dto.Files.Count > 0)
            {
                var file = dto.Files.FirstOrDefault(f => f != null && f.Length > 0);
                if (file != null)
                    existingImage.Url = await _imageUploadService.UploadImageAsync(file);
            }
            savedImages.Add(existingImage);
        }

        if (dto.IsCover)
        {
            var savedIds = savedImages.Select(x => x.Id).ToList();
            var otherCovers = await _productImageRepository.GetAllAsync(
                x => x.ProductId == dto.ProductId && !savedIds.Contains(x.Id) && x.IsCover, cancellationToken: cancellationToken);
            foreach (var cover in otherCovers)
            {
                cover.IsCover = false;
                await _productImageRepository.UpdateAsync(cover, cancellationToken);
            }
        }

        if (existingImage is null)
        {
            foreach (var image in savedImages)
                await _productImageRepository.AddAsync(image, cancellationToken);
        }
        else
        {
            await _productImageRepository.UpdateAsync(savedImages[0], cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync(ProductCacheKey(dto.ProductId), cancellationToken);

        var result = _mapper.Map<List<GetProductImageDto>>(savedImages);
        return ApiResponse<List<GetProductImageDto>>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (image is null)
            throw new NotFoundException(nameof(ProductImage), id);

        await _productImageRepository.HardDeleteAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync(ProductCacheKey(image.ProductId), cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}
