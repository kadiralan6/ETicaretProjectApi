using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Identity.Domain.DTOs;
using ETicaretAPI.Services.Identity.Domain.Entities;
using ETicaretAPI.Services.Identity.Domain.Interfaces.Repositories;

namespace ETicaretAPI.Services.Identity.Application.Services.AddressService;

public class AddressManager : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddressManager(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<AddressDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var addresses = await _addressRepository.GetAllWithAsNoTrackingAsync(cancellationToken: cancellationToken);
        var result = addresses.Select(MapToDto).ToList();
        return ApiResponse<List<AddressDto>>.Success(result);
    }

    public async Task<ApiResponse<AddressDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var address = await _addressRepository.GetWithAsNoTrackingAsync(a => a.Id == id, cancellationToken: cancellationToken);

        if (address is null)
            throw new NotFoundException(nameof(Address), id);

        return ApiResponse<AddressDto>.Success(MapToDto(address));
    }

    public async Task<ApiResponse<AddressDto>> CreateAsync(CreateAddressDto dto, CancellationToken cancellationToken = default)
    {
        var address = new Address
        {
            Title = dto.Title,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            City = dto.City,
            District = dto.District,
            FullAddress = dto.FullAddress,
            PostalCode = dto.PostalCode,
            IsDefault = dto.IsDefault
        };

        await _addressRepository.AddAsync(address, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<AddressDto>.Success(MapToDto(address), statusCode: 201);
    }

    public async Task<ApiResponse<AddressDto>> UpdateAsync(int id, UpdateAddressDto dto, CancellationToken cancellationToken = default)
    {
        var address = await _addressRepository.GetAsync(a => a.Id == id, cancellationToken: cancellationToken);

        if (address is null)
            throw new NotFoundException(nameof(Address), id);

        address.Title = dto.Title;
        address.FullName = dto.FullName;
        address.PhoneNumber = dto.PhoneNumber;
        address.City = dto.City;
        address.District = dto.District;
        address.FullAddress = dto.FullAddress;
        address.PostalCode = dto.PostalCode;
        address.IsDefault = dto.IsDefault;

        await _addressRepository.UpdateAsync(address, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<AddressDto>.Success(MapToDto(address));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var address = await _addressRepository.GetAsync(a => a.Id == id, cancellationToken: cancellationToken);

        if (address is null)
            throw new NotFoundException(nameof(Address), id);

        await _addressRepository.SoftDeleteAsync(address, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    private static AddressDto MapToDto(Address address) => new()
    {
        Id = address.Id,
        Title = address.Title ?? string.Empty,
        FullName = address.FullName ?? string.Empty,
        PhoneNumber = address.PhoneNumber ?? string.Empty,
        City = address.City ?? string.Empty,
        District = address.District ?? string.Empty,
        FullAddress = address.FullAddress ?? string.Empty,
        PostalCode = address.PostalCode ?? string.Empty,
        IsDefault = address.IsDefault
    };
}
