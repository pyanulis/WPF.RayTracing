using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing.Model.Strategies
{
    internal class ImageBlockGenerateStrategy : GenerateStrategyAbstract
    {public override IEnumerable<(int, int)> GetNextPage()
        {
            int pageEnd = m_pageNum == m_threadCount - 1 ? m_imgHeight - 1 : m_pageStart + m_pageSize - 1;

            //Shuffle to make image fade in
            IEnumerable<(int, int)> page = Shuffle(m_pageStart, pageEnd, out int total);
            m_pageNum++;
            m_pageStart += m_pageSize;
            return page;
        }
    }
}
