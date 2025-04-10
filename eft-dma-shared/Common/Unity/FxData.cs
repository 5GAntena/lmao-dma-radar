using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eft_dma_shared.Common.Unity
{
    public class FxData
    {
        public bool FxState { get; set; }
        public float FxVolume { get; set; }

        public FxData(bool fxState, float fxVolume)
        {
            FxState = fxState;
            FxVolume = fxVolume;
        }
    }
}
