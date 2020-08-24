using AutoMapper;

namespace StockControl.Orders.Application
{

    public class DraftOrderProfile : Profile
        {
            public DraftOrderProfile()
            {
                CreateMap<DraftOrder, DraftOrderViewModel>();
                CreateMap<DraftOrderItem, DraftOrderItemViewModel>()
                    .ForMember(
                    item => item.ProductName,
                    opt => opt.MapFrom(
                        src => src.LineItem.Product.Name));
            }
        }
    }
