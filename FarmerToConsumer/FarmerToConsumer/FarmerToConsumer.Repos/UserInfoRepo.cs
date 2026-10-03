using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;

namespace FarmerToConsumer.Repos;

public class UserInfoRepo(FtcDbContext context)
{
    public Result<List<UserInfo>> GetAll()
    {
        var result = new Result<List<UserInfo>>()
        {
            Data = new List<UserInfo>()
        };
        try
        {
            result.Data = context.UserInfos.ToList();
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }
    public Result<UserInfo> GetById(int id)
    {
        var result = new Result<UserInfo>();
        try
        {
            result.Data = context.UserInfos.Find(id);
            if (result.Data == null)
            {
                result.HasError = true;
                result.Message = "Invalid Id";
            }
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }
    public Result<UserInfo> Save(UserInfo model)
    {
        var result = new Result<UserInfo>();

        try
        {
            var objToSave = context.UserInfos.Find(model.ID);
            var isNewUser = objToSave == null;
            var isEmailChanging = isNewUser ||
                !string.Equals(objToSave!.Email, model.Email, StringComparison.OrdinalIgnoreCase);

            if (isEmailChanging && context.UserInfos.Any(e =>
                    e.Email.ToLower() == model.Email.ToLower() && e.ID != model.ID))
            {
                result.HasError = true;
                result.Message = "Email already exists.";
                return result;
            }

            if (objToSave == null)
            {
                objToSave = new UserInfo();
                context.UserInfos.Add(objToSave);
            }

            objToSave.Name = model.Name;
            objToSave.Email = model.Email;
            objToSave.Password = model.Password;
            objToSave.Role = model.Role;
            objToSave.Phone = model.Phone;
            objToSave.Village = model.Village;
            objToSave.District = model.District;
            objToSave.Address = model.Address;
            context.SaveChanges();

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

    public Result<UserInfo> Authenticate(string login, string password)
    {
        var result = new Result<UserInfo>();
        try
        {
            login = login?.Trim() ?? "";
            password = password?.Trim() ?? "";

            result.Data = context.UserInfos.FirstOrDefault(e =>
                ((e.Email != null && e.Email.ToLower() == login.ToLower()) ||
                 (e.Phone != null && e.Phone.Trim() == login)) &&
                e.Password == password);

            if (result.Data == null)
            {
                result.HasError = true;
                result.Message = "Invalid Credential";
            }
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
            var objToDelete = context.UserInfos.Find(id);
            if (objToDelete == null)
            {
                result.HasError = true;
                result.Message = "Invalid Event Type Id.";
                return result;
            }

            context.UserInfos.Remove(objToDelete);
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

    public Result<bool> ChangePassword(int userId, string currentPassword, string newPassword)
    {
        var result = new Result<bool>();

        try
        {
            var user = context.UserInfos.Find(userId);
            if (user == null)
            {
                result.HasError = true;
                result.Message = "Invalid Id";
                return result;
            }

            if (user.Password != currentPassword)
            {
                result.HasError = true;
                result.Message = "Current password is incorrect.";
                return result;
            }

            user.Password = newPassword;
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
