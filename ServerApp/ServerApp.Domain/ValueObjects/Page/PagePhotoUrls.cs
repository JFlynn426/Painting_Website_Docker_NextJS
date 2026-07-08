namespace ServerApp.Domain.ValueObjects.Page;

using System.Collections.ObjectModel;

public class PagePhotoUrls : IList<string>
{
    public const int MaxLengthPerUrl = 2000;
    public const int MaxUrls = 20;

    private readonly List<string> _urls;

    public PagePhotoUrls()
    {
        _urls = new List<string>();
    }

    public PagePhotoUrls(IEnumerable<string> urls)
    {
        _urls = urls.ToList();
        Validate();
    }

    private void Validate()
    {
        if (_urls.Count > MaxUrls)
        {
            throw new ArgumentException($"Photo URLs cannot exceed {MaxUrls} items.");
        }

        for (int i = 0; i < _urls.Count; i++)
        {
            if (_urls[i].Length > MaxLengthPerUrl)
            {
                throw new ArgumentException($"Photo URL at index {i} cannot exceed {MaxLengthPerUrl} characters.");
            }
        }
    }

    public string this[int index]
    {
        get => _urls[index];
        set => _urls[index] = value;
    }

    public int Count => _urls.Count;

    public bool IsReadOnly => true;

    public IEnumerator<string> GetEnumerator() => _urls.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _urls.GetEnumerator();

    public void Add(string item) => _urls.Add(item);

    public void Clear() => _urls.Clear();

    public bool Contains(string item) => _urls.Contains(item);

    public void CopyTo(string[] array, int arrayIndex) => _urls.CopyTo(array, arrayIndex);

    public bool Remove(string item) => _urls.Remove(item);

    public void RemoveAt(int index) => _urls.RemoveAt(index);

    public int IndexOf(string item) => _urls.IndexOf(item);

    public void Insert(int index, string item) => _urls.Insert(index, item);

    public static implicit operator PagePhotoUrls(string[] urls) => new(urls);

    public static PagePhotoUrls? FromNullable(IEnumerable<string>? urls)
    {
        if (urls == null)
        {
            return null;
        }
        if (!urls.Any())
        {
            return new PagePhotoUrls();
        }
        return new PagePhotoUrls(urls);
    }

    public static PagePhotoUrls? FromNullable(string?[]? urls)
    {
        if (urls == null)
        {
            return null;
        }
        var filtered = urls.Where(u => u != null).Select(u => u!).ToList();
        if (filtered.Count == 0)
        {
            return new PagePhotoUrls();
        }
        return new PagePhotoUrls(filtered);
    }
}
