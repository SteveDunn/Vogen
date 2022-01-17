using System.Runtime.CompilerServices;
using Dapper;

namespace Testbench;

    public static class DapperTypeHandlers
    {
        [ModuleInitializer]
        public static void AddHandlers()
        {
            //SqlMapper.AddTypeHandler(typeof(DapperDateTimeVo), new DapperDateTimeVo.DapperTypeHandler());
        }
    }

