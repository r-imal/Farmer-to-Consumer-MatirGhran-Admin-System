using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;

namespace FarmerToConsumer.Repos
{
    public class FarmerRepo(FtcDbContext context)
    {
        public Result<List<Farmer>> GetAll()
        {
            var result = new Result<List<Farmer>>()
            {
                Data = new List<Farmer>()
            };

            try
            {
                result.Data = context.Farmers.ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<List<Farmer>> GetAllByAgentId(int agentId)
        {
            var result = new Result<List<Farmer>>()
            {
                Data = new List<Farmer>()
            };

            try
            {
                result.Data = context.Farmers
                    .Where(e => e.AgentId == agentId)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Farmer> GetById(int id)
        {
            var result = new Result<Farmer>();

            try
            {
                result.Data = context.Farmers.Find(id);

                if (result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Farmer Id.";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Farmer> Save(Farmer model)
        {
            var result = new Result<Farmer>();

            try
            {
                if (context.Farmers.Any(e => e.Contact == model.Contact && e.ID != model.ID))
                {
                    result.HasError = true;
                    result.Message = "Contact already exist.";
                    return result;
                }

                var objToSave = context.Farmers.Find(model.ID);

                if (objToSave == null)
                {
                    objToSave = new Farmer();
                    context.Farmers.Add(objToSave);
                }

                objToSave.AgentId = model.AgentId;
                objToSave.Name = model.Name;
                objToSave.Village = model.Village;
                objToSave.District = model.District;
                objToSave.Contact = model.Contact;

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
                var objToDelete = context.Farmers.Find(id);

                if (objToDelete == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Farmer Id.";
                    return result;
                }

                context.Farmers.Remove(objToDelete);
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
