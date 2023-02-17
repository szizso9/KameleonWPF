using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kameleon2.Persistence
{
    public interface IKameleonDataAccess
    {
        Task<KameleonMap> LoadAsync(string path);

        Task SaveAsync(string path, KameleonMap table);
    }
}
