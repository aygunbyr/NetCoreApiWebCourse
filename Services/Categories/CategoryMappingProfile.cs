using App.Repositories.Categories;
using App.Services.Categories.Create;
using App.Services.Categories.Dto;
using App.Services.Categories.Update;
using AutoMapper;

namespace App.Services.Categories
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<CategoryDto, Category>().ReverseMap();

            CreateMap<Category, CategoryWithProductsDto>().ReverseMap();
            
            CreateMap<CreateCategoryRequest, Category>().ForMember(
                destination => destination.Name,
                options => options.MapFrom(source => source.Name.ToLowerInvariant()));

            CreateMap<UpdateCategoryRequest, Category>().ForMember(
                destination => destination.Name,
                options => options.MapFrom(source => source.Name.ToLowerInvariant()));
        }
    }
}
