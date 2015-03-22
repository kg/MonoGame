using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework.Graphics {
    public static class JSILHelpers {
        // will be proxied out
        public static T[] GetBytes<T>(T[] data)
            where T : struct {
            return data;
        }
    }
}
