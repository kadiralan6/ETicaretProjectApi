using AutoMapper;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Payment.Domain.DTOs.PaymentDetailDtos;
using ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;
using ETicaretAPI.Services.Payment.Domain.Entities;

namespace ETicaretAPI.Services.Payment.Application.Mappings.AutoMapper.Profiles;

public class PaymentTransactionProfile : Profile
{
    public PaymentTransactionProfile()
    {
        CreateMap<PaymentTransaction, CreatePaymentTransactionDto>().ReverseMap();

        CreateMap<PaymentTransaction, UpdatePaymentTransactionDto>().ReverseMap();

        CreateMap<PaymentTransaction, GetPaymentTransactionDto>().ReverseMap();

        CreateMap<PaymentDetail, GetPaymentDetailDto>().ReverseMap();

        CreateMap<PagedResult<PaymentTransaction>, PagedResult<GetPaymentTransactionDto>>().ReverseMap();
    }
}
