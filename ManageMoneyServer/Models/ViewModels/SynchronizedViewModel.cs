using System.Collections.Generic;

namespace ManageMoneyServer.Models.ViewModels
{
    public class SynchronizedViewModel<T> where T : class
    {
        public IEnumerable<T> Added { get; set; }
        public IEnumerable<T> Removed { get; set; }
        public IEnumerable<T> Updated { get; set; }
    }
}
