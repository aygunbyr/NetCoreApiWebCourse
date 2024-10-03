using App.Repositories.Products;
using App.Services.Products.Create;
using App.Services.Products.Update;
using AutoMapper;

namespace App.Services.Products
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<CreateProductRequest, Product>().ForMember(
                destination => destination.Name,
                options => options.MapFrom(source => source.Name.ToLowerInvariant()));

            CreateMap<UpdateProductRequest, Product>().ForMember(
                destination => destination.Name,
                options => options.MapFrom(source => source.Name.ToLowerInvariant()));
        }
    }
}
