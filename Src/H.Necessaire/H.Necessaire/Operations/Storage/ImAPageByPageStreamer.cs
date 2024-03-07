using System.Collections.Generic;

namespace H.Necessaire
{
    public interface ImAPageByPageStreamer<TEntity>
    {
        IEnumerable<Page<TEntity>> StreamAllPageByPage(int pageSize = 25);
    }
}
