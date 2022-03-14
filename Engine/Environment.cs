using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Engine
{
    
    /// <summary>
    /// Класс окружения. Используется для получения внешних данных. (Мышь, Клавиатура, Массивы и т.д.)
    /// </summary>
    
    public static class Environment
    {
        /// <summary>
        /// Массив нажатых клавиш
        /// </summary>
        public static List<Key> keys = new List<Key>();

        /// <summary>
        /// Получение всех нажатых клавиш
        /// </summary>
        /// <returns>Лист всех нажатых клавиш</returns>
        public static List<Key> GetInput() //Получить все нажате клавиши (столько сколько поддерживает клавиатура)
        {
            return GetDownKeys().ToList();
        }

        /// <summary>
        /// Проверяет нажатие заданной клавиши
        /// </summary>
        /// <param name="key">Клавиша для проверки</param>
        /// <returns>True, если заданная клавиша была нажата</returns>
        public static bool GetKey(Key key) //Проверить есть ли кнопка в нажатых клавишах
        {
            return GetDownKeys().ToList().Contains(key);
        }

        private static readonly byte[] DistinctVirtualKeys = Enumerable //Ищим кнопки и преабразуем
                .Range(0, 256)
                .Select(KeyInterop.KeyFromVirtualKey)
                .Where(item => item != Key.None)
                .Distinct()
                .Select(item => (byte)KeyInterop.VirtualKeyFromKey(item))
                .ToArray();


        private static IEnumerable<Key> GetDownKeys() // Возвращаем список из кнопок.
        {
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            var downKeys = new List<Key>();
            for (var index = 0; index < DistinctVirtualKeys.Length; index++)
            {
                var virtualKey = DistinctVirtualKeys[index];
                if ((keyboardState[virtualKey] & 0x80) != 0)
                {
                    downKeys.Add(KeyInterop.KeyFromVirtualKey(virtualKey));
                }
            }

            return downKeys;
        }
        [DllImport("user32.dll")] //Работа с ОС
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetKeyboardState(byte[] keyState);
    }
}
