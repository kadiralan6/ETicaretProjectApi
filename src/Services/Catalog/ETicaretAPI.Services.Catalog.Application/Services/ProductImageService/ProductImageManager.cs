using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
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

    public ProductImageManager(IProductImageRepository productImageRepository, IImageUploadService imageUploadService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _productImageRepository = productImageRepository;
        _imageUploadService = imageUploadService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

    public async Task<ApiResponse<GetProductImageDto>> CreateAsync(CreateProductImageDto dto, CancellationToken cancellationToken = default)
    {
        var image = _mapper.Map<ProductImage>(dto);

        if (dto.Files != null && dto.Files.Count > 0)
        {
            var uploadedUrls = new List<string>();
            foreach (var file in dto.Files)
            {
                if (file != null && file.Length > 0)
                {
                    var uploadedUrl = await _imageUploadService.UploadImageAsync(file);
                    uploadedUrls.Add(uploadedUrl);
                }
            }
            image.Url = uploadedUrls.FirstOrDefault(); // Set the first uploaded image as the main URL
        }

        if (image.IsCover)
        {
            var otherCovers = await _productImageRepository.GetAllAsync(
                x => x.ProductId == image.ProductId && x.IsCover, cancellationToken: cancellationToken);
            foreach (var cover in otherCovers)
            {
                cover.IsCover = false;
                await _productImageRepository.UpdateAsync(cover, cancellationToken);
            }
        }

        await _productImageRepository.AddAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetProductImageDto>(image);
        return ApiResponse<GetProductImageDto>.Success(result);
    }

    public async Task<ApiResponse<GetProductImageDto>> UpdateAsync(UpdateProductImageDto dto, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        if (image is null)
            throw new NotFoundException(nameof(ProductImage), dto.Id);

        _mapper.Map(dto, image);

        if (dto.Files != null && dto.Files.Count > 0)
        {
            var uploadedUrls = new List<string>();
            foreach (var file in dto.Files)
            {
                if (file != null && file.Length > 0)
                {
                    var uploadedUrl = await _imageUploadService.UploadImageAsync(file);
                    uploadedUrls.Add(uploadedUrl);
                }
            }
            image.Url = uploadedUrls.FirstOrDefault(); // Set the first uploaded image as the main URL
        }

        if (image.IsCover)
        {
            var otherCovers = await _productImageRepository.GetAllAsync(
                x => x.ProductId == image.ProductId && x.Id != image.Id && x.IsCover, cancellationToken: cancellationToken);
            foreach (var cover in otherCovers)
            {
                cover.IsCover = false;
                await _productImageRepository.UpdateAsync(cover, cancellationToken);
            }
        }

        await _productImageRepository.UpdateAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetProductImageDto>(image);
        return ApiResponse<GetProductImageDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (image is null)
            throw new NotFoundException(nameof(ProductImage), id);

        await _productImageRepository.HardDeleteAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}
