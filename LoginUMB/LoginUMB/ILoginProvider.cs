using System.Threading.Tasks;

namespace LoginUMB
{
    public interface ILoginProvider
    {
        Task<string> LoginAsync();
    }
}
