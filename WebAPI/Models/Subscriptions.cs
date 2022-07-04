namespace WebAPI.Models;

public class Subscriptions
{
    private static IDictionary<string, IList<string>> _collection;

    public IDictionary<string, IList<string>> Collection
    {
        get
        {
            if (_collection == null)
                _collection = new Dictionary<string, IList<string>>();

            return _collection;
        }
    }
}