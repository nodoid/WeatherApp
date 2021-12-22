using System.Collections.Generic;

namespace WeatherApp.Interfaces
{
    public interface IRepository
    {
        void SaveData<T>(T toStore);

        void SaveListData<T>(List<T> toStore);

        List<T> GetList<T>(int top = 0) where T : class, new();

        List<T> GetList<T, TU>(string para, TU val, int top = 0) where T : class, new();

        T? GetData<T>() where T : class, new();

        T? GetData<T, TU>(string para, TU val) where T : class, new();

        T? GetData<T, TU, TV>(string para1, TU val1, string para2, TV val2) where T : class, new();

        void Delete<T>(T stored);

        void DeleteAll();

        int GetID<T>() where T : class, new();

        int Count<T>() where T : class, new();

        int Count<T, U>(string p, U val) where T : class, new();
    }
}
