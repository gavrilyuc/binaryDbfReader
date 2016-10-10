using System.Collections.Generic;

namespace System.IO.DbfStream
{
    /// <summary>
    /// Представление контекста
    /// </summary>
    public interface IContextReader : IDisposable
    {
        /// <summary>
        /// Получить данные
        /// </summary>
        /// <typeparam name="T">Тип контракта</typeparam>
        /// <returns>Данные из контекста</returns>
        IEnumerable<T> GetData<T>() where T : class, new();
        /// <summary>
        /// Получить данные
        /// </summary>
        /// <param name="objectType">Тип контракта</param>
        /// <returns>Данные из контекста</returns>
        IEnumerable<object> GetData(Type objectType);
    }
}