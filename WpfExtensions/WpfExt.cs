using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
#if Net45

#endif

namespace Tyrrrz.WpfExtensions
{
    /// <summary>
    /// WPF extension methods for rapid development
    /// </summary>
    public static class WpfExt
    {
        /// <summary>
        /// Invokes the delegate on the dispatcher thread if necessary
        /// </summary>
        public static void InvokeSafe(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.Invoke(action);
        }

        /// <summary>
        /// Invokes the delegate on the dispatcher thread if necessary
        /// </summary>
        public static T InvokeSafe<T>(this Dispatcher dispatcher, Func<T> func)
        {
            if (dispatcher.CheckAccess())
                return func();
#if Net45
            return dispatcher.Invoke(func);
#else
            return (T) dispatcher.Invoke(func);
#endif
        }

#if Net45
        /// <summary>
        /// Invokes the delegate asynchronously on the dispatcher thread if necessary or synchronously if not
        /// </summary>
        public static async Task InvokeSafeAsync(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else
                await dispatcher.InvokeAsync(action);
        }

        /// <summary>
        /// Invokes the delegate asynchronously on the dispatcher thread if necessary or synchronously if not
        /// </summary>
        public static async Task<T> InvokeSafeAsync<T>(this Dispatcher dispatcher, Func<T> func)
        {
            if (dispatcher.CheckAccess())
                return func();
            return await dispatcher.InvokeAsync(func);
        }
#endif

        /// <summary>
        /// Safely shutdowns an application from dispatcher thread
        /// </summary>
        public static void ShutdownSafe(this Application application, int code)
        {
            InvokeSafe(application.Dispatcher, () => application.Shutdown(code));
        }

        /// <summary>
        /// Safely shutdowns an application from dispatcher thread
        /// </summary>
        public static void ShutdownSafe(this Application application, Enum code)
        {
            object codeActualValue = Convert.ChangeType(code, code.GetTypeCode());
            int codeInt = (int?) codeActualValue ?? 0;
            InvokeSafe(application.Dispatcher, () => application.Shutdown(codeInt));
        }

        /// <summary>
        /// Safely shutdowns an application from dispatcher thread
        /// </summary>
        public static void ShutdownSafe(this Application application)
        {
            InvokeSafe(application.Dispatcher, application.Shutdown);
        }

        /// <summary>
        /// Shows a message box safely from the dispatcher of the given window
        /// </summary>
        public static MessageBoxResult ShowMessage(this Window window,
            string title = null, string message = null,
            MessageBoxButton buttonSet = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None,
            MessageBoxResult defaultResult = default(MessageBoxResult))
        {
            var func =
                new Func<MessageBoxResult>(() => MessageBox.Show(window, message, title, buttonSet, image, defaultResult));
            return InvokeSafe(window.Dispatcher, func);
        }

#if Net45
        /// <summary>
        /// Shows a message box asynchronously from the dispatcher of the given window
        /// </summary>
        public static async Task<MessageBoxResult> ShowMessageAsync(this Window window,
            string title = null, string message = null,
            MessageBoxButton buttonSet = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None,
            MessageBoxResult defaultResult = default(MessageBoxResult))
        {
            var func =
                new Func<MessageBoxResult>(() => MessageBox.Show(window, message, title, buttonSet, image, defaultResult));
            return await InvokeSafeAsync(window.Dispatcher, func);
        }
#endif
    }
}