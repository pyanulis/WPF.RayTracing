using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing.Model.Strategies
{
    internal class FlatGenerateStrategy : GenerateStrategyAbstract
    {
        private IEnumerable<(int, int)> m_pixels;

        public override IEnumerable<(int, int)> GetNextPage()
        {
            IEnumerable<(int, int)> page = new List<(int, int)>(m_pixels.Skip(m_pageStart).Take(m_pageSize));
            m_pageStart += m_pageSize;

            return page;
        }

        public override void Init(int threadCount, int imgHeight, int imgWidth)
        {
            base.Init(threadCount, imgHeight, imgWidth);

            m_pixels = Shuffle(out int total);
            m_pageSize = total / threadCount;
        }
    }
}
