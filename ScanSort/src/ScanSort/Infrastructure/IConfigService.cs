namespace ScanSort.Infrastructure;

public interface IConfigService
{
    AppSettings Load();
    void Save(AppSettings settings);
}
