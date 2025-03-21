namespace Ironwall.Dotnet.Framework.Models.Communications.Accounts
{
    public interface IAccountIdCheckRequest : IBaseMessageModel
    {
        string IdUser { get; set; }
    }
}