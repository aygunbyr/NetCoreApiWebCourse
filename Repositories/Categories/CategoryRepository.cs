using Microsoft.EntityFrameworkCore;

namespace App.Repositories.Categories
{
    public class CategoryRepository(AppDbContext context) : GenericRepository<Category>(context), ICategoryRepository
    {
        public Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            var categoryWithProducts = context.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
            return categoryWithProducts;
        }

        public IQueryable<Category> GetCategoryWithProducts()
        {
            var categoryByProducts = context.Categories.Include(x => x.Products).AsQueryable();
            return categoryByProducts;
        }
    }
}
