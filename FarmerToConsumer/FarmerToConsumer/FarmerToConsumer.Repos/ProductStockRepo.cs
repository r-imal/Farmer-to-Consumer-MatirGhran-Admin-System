using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;
using Microsoft.EntityFrameworkCore;

namespace FarmerToConsumer.Repos
{
    public class ProductStockRepo(FtcDbContext context)
    {
        public Result<List<ProductStock>> GetAll()
        {
            var result = new Result<List<ProductStock>>()
            {
                Data = new List<ProductStock>()
            };

            try
            {
                result.Data = context.ProductStocks
                    .Include(e => e.Product)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<List<ProductStock>> GetAllByAgentId(int agentId)
        {
            var result = new Result<List<ProductStock>>()
            {
                Data = new List<ProductStock>()
            };

            try
            {
                result.Data = context.ProductStocks
                    .Include(e => e.Product)
                    .Where(e => e.Product != null && e.Product.AddedByAgent == agentId)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<ProductStock> GetById(int id)
        {
            var result = new Result<ProductStock>();

            try
            {
                result.Data = context.ProductStocks.Find(id);

                if (result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Product Stock Id.";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<ProductStock> Save(ProductStock model)
        {
            var result = new Result<ProductStock>();

            try
            {
                if (model.ProductID <= 0)
                {
                    result.HasError = true;
                    result.Message = "Please select product.";
                    return result;
                }

                if (model.Price <= 0)
                {
                    result.HasError = true;
                    result.Message = "Price must be greater than zero.";
                    return result;
                }

                if (model.Quantity <= 0)
                {
                    result.HasError = true;
                    result.Message = "Quantity must be greater than zero.";
                    return result;
                }

                if (context.ProductStocks.Any(e => e.ProductId == model.ProductID && e.ID != model.ID))
                {
                    result.HasError = true;
                    result.Message = "Stock already exists for this product.";
                    return result;
                }

                var objToSave = context.ProductStocks.Find(model.ID);

                if (objToSave == null)
                {
                    objToSave = new ProductStock();
                    context.ProductStocks.Add(objToSave);
                }

                objToSave.ProductId = model.ProductID;
                objToSave.Price = model.Price;
                objToSave.Quantity = model.Quantity;

                context.SaveChanges();
                result.Data = objToSave;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<bool> Delete(int id)
        {
            var result = new Result<bool>();

            try
            {
                var objToDelete = context.ProductStocks.Find(id);

                if (objToDelete == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Product Stock Id.";
                    return result;
                }

                context.ProductStocks.Remove(objToDelete);
                context.SaveChanges();
                result.Data = true;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }
    }
}
