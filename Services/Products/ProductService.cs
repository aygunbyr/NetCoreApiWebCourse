using App.Repositories;
using App.Repositories.Products;
using App.Services.ExceptionHandlers;
using App.Services.Products.Create;
using App.Services.Products.Update;
using App.Services.Products.UpdateStock;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Products
{
    public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper) : IProductService
    {
        public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
        {
            var products = await productRepository.GetTopPriceProductsAsync(count);

            #region manuel mapping
            //var productsAsDto = products.Select(p => new ProductDto(p.Id,p.Name,p.Price,p.Stock)).ToList();
            #endregion

            var productsAsDto = mapper.Map<List<ProductDto>>(products);

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllAsync()
        {
            var products = await productRepository.GetAll().ToListAsync();

            #region manuel mapping
            //var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList(); 
            #endregion

            var productsAsDto = mapper.Map<List<ProductDto>>(products);

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            var products = await productRepository.GetAll().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            #region manuel mapping
            //var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList(); 
            #endregion

            var productsAsDto = mapper.Map<List<ProductDto>>(products);

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }

        public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if(product is null)
            {
                return ServiceResult<ProductDto?>.Fail("Product not found", HttpStatusCode.NotFound);
            }

            #region manuel mapping
            //var productAsDto = new ProductDto(product.Id, product.Name, product.Price, product.Stock);
            #endregion

            var productAsDto = mapper.Map<ProductDto>(product);

            return ServiceResult<ProductDto>.Success(productAsDto)!;
        }

        public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            //throw new CriticalException("kritik seviye bir hata meydana geldi");

            bool isProductNameExist = await productRepository.Where(x => x.Name == request.Name).AnyAsync();

            if(isProductNameExist)
            {
                return ServiceResult<CreateProductResponse>.Fail("Product already exists", HttpStatusCode.BadRequest);
            }

            #region manuel mapping
            //var product = new Product()
            //{
            //    Name = request.Name,
            //    Price = request.Price,
            //    Stock = request.Stock
            //};
            #endregion

            var product = mapper.Map<Product>(request);

            await productRepository.AddAsync(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id), $"api/products/{product.Id}");
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
        {
            //var product = await productRepository.GetByIdAsync(id);

            //if(product is null)
            //{
            //    return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            //}

            bool isProductNameExist = await productRepository.Where(x => x.Name == request.Name && x.Id != id /*product.Id*/).AnyAsync();

            if(isProductNameExist)
            {
                return ServiceResult.Fail("Product already exists", HttpStatusCode.BadRequest);
            }

            //product.Name = request.Name;
            //product.Price = request.Price;
            //product.Stock = request.Stock;

            // elimde product nesnesi var, yeniden oluşturmuyorum, bu yüzden Map methoduna generic sınıf vermedim.
            // diğer örneklerde yeniden nesne oluşturulduğu için mapper.Map<Product>(request) şeklinde kullanıyordum.
            // yani bu kod var olan nesneme özellikleri assign ediyor, yukarıdaki satırların yaptığını yapıyor
            // ekstradan request'ten gelen product name'i küçük harflere dönüştürüyor (mapper'da ayarlanan şekilde)
            
            //product = mapper.Map(request, product);
            // artık filter kullanıyorum, product getbyid methodunu kullanmıyorum.
            var product = mapper.Map<Product>(request);
            // Id assign etmezsek update yapmak yerine yeni kayıt ekliyor!
            product.Id = id;

            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request)
        {
            var product = await productRepository.GetByIdAsync(request.ProductId);

            if(product is null)
            {
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }

            product.Stock = request.Quantity;
            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            //if (product is null)
            //{
            //    return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            //}

            // filterdan geçtiğine göre null olamaz
            productRepository.Delete(product!);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
    }
}
