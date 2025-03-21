
namespace Ironwall.Dotnet.Libraries.Db.Utils;

public interface IExcelImporter
{
    Task<bool> ImportExcelToDbAsync(string filePath, CancellationToken token = default);
}