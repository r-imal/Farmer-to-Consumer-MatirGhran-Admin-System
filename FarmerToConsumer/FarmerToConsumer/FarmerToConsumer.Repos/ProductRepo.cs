using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;
using Microsoft.EntityFrameworkCore;

namespace FarmerToConsumer.Repos
{
    public class ProductRepo(FtcDbContext context)
    {
        public Result<List<Product>> GetAll()
        {
            var result = new Result<List<Product>>()
            {
                Data = new List<Product>()
            };

            try
            {
                result.Data = context.Products
                    .Include(e => e.Farmer)
                    .Include(e => e.Agent)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<List<Product>> GetAllByAgentId(int agentId)
        {
            var result = new Result<List<Product>>()
            {
                Data = new List<Product>()
            };

            try
            {
                result.Data = context.Products
                    .Include(e => e.Farmer)
                    .Include(e => e.Agent)
                    .Where(e => e.AddedByAgent == agentId)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Product> GetById(int id)
        {
            var result = new Result<Product>();

            try
            {
                result.Data = context.Products
                    .Include(e => e.Farmer)
                    .Include(e => e.Agent)
                    .FirstOrDefault(e => e.ID == id);

                if (result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Product Id.";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Product> Save(Product model)
        {
            var result = new Result<Product>();

            try
            {
                if (model.FarmerId <= 0)
                {
                    result.HasError = true;
                    result.Message = "Please select farmer.";
                    return result;
                }

                var farmer = context.Farmers.Find(model.FarmerId);

                if (farmer == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Farmer.";
                    return result;
                }

                if (context.Products.Any(e =>
                        e.Name.ToLower() == model.Name.ToLower()
                        && e.FarmerId == model.FarmerId
                        && e.ID != model.ID))
                {
                    result.HasError = true;
                    result.Message = "Product already exist for this farmer.";
                    return result;
                }

                var objToSave = context.Products.Find(model.ID);

                if (objToSave == null)
                {
                    objToSave = new Product();
                    context.Products.Add(objToSave);
                }

                objToSave.FarmerId = model.FarmerId;

                // Farmer jei Agent-er under-e, Product automatically oi Agent-er under-e save hobe
                objToSave.AddedByAgent = farmer.AgentId;

                objToSave.Name = model.Name;
                objToSave.Category = model.Category;

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
                var objToDelete = context.Products.Find(id);

                if (objToDelete == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Product Id.";
                    return result;
                }

                context.Products.Remove(objToDelete);
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