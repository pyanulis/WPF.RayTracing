using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing.Model.Strategies
{
    internal abstract class GenerateStrategyAbstract
    {
        protected int m_threadCount;
        protected int m_imgHeight;
        protected int m_imgWidth;

        protected int m_pageSize;
        protected int m_pageStart;
        protected int m_pageNum;

        public virtual void Init(int threadCount, int imgHeight, int imgWidth)
        {
            m_threadCount = threadCount;
            m_imgHeight = imgHeight;
            m_imgWidth = imgWidth;

            m_pageSize = m_imgHeight / m_threadCount;
            m_pageStart = 0;
            m_pageNum = 0;
        }

        public abstract IEnumerable<(int, int)> GetNextPage();

        protected IEnumerable<(int, int)> Shuffle(out int total)
        {
            return Shuffle(0, m_imgHeight - 1, out total);
        }

        protected IEnumerable<(int, int)> Shuffle(int jMin, int jMax, out int total)
        {
            Random random = new();

            List<int> vert = new();
            List<int> hor = new();
            for (int j = jMin; j <= jMax; ++j)
            {
                vert.Add(j);
            }
            for (int i = 0; i < m_imgWidth; ++i)
            {
                hor.Add(i);
            }

            total = vert.Count * hor.Count;

            var coords = vert.SelectMany(j => hor.Select<int, (int, int)>(i => new(j, i)));
            // OrderBy must execute only once to make distinct pieces
            List<(int, int)> list = new(coords.OrderBy(x => random.Next()));
            return list;
        }
    }
}
