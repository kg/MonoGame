using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework.Graphics {
    // these are currently implemented in SampleFNA proxies
    public static class JSILHelpers {
        internal static void BufferSubData (string p, int elementSizeInBytes, int offsetInBytes, Array data, int startIndex, int elementCount) {
            throw new NotImplementedException();
        }
    }
}

namespace Microsoft.Xna.Framework.Audio {
    public static class JSILHelpers {
        internal static void alBufferData(uint buffer, int format, Array data, int length, uint sampleRate) {
            throw new NotImplementedException();
        }
    }
}
