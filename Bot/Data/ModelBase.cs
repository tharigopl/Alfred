using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Data
{
    public interface IModelBase
    {
        string Id { get; set; }
    }

    public class ModelBase : IModelBase
    {
        public string Id { get; set; }
    }
}
