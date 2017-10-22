using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Xunit;

namespace website_performance.Tests.Unit.Helpers
{
    public class TestTheoryData<T> : TheoryData<T>
    {
        public TestTheoryData(IReadOnlyCollection<T> data)
        {
            Contract.Assert(data != null && data.Any());

            foreach (var t in data)
                Add(t);
        }
    }
}