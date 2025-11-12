using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programa1.Util
{
    public class Timer
    {
        private int currentFrame = 0;
        private readonly int threshold;

        public Timer(int threshold)
        {
            this.threshold = threshold;
        }

        public bool Tick()
        {
            currentFrame++;
            if (currentFrame >= threshold)
            {
                currentFrame = 0;
                return true; // tiempo cumplido
            }
            return false;
        }

        public void Reset()
        {
            currentFrame = 0;
        }
    }
}

