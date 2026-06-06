# RevitLogger

`RevitLogger` - небольшая библиотека для Revit-плагинов на `.NET Framework 4.8`, которая решает две задачи:

- пишет текстовый лог в файл;
- показывает простое WinForms-окно для быстрой отладки строковых данных.

Проект удобно использовать как внутреннюю утилиту в своих Revit-надстройках, когда нужен понятный лог без внешних зависимостей.

## Что делает библиотека

### 1. Файловый логгер

Статический класс `Logger` умеет:

- очищать лог при старте;
- писать сообщения уровней `Debug`, `Info`, `Warning`, `Error`, `Critical`;
- автоматически добавлять время, уровень, имя файла и номер строки вызова;
- писать информацию об окружении через `Init(...)`;
- писать исключения через `Exception(...)`.

Формат строки лога:

```text
2026-06-06 18:30:15.123 | INFO     | Command.cs:42 | Export started
```

Путь лога по умолчанию:

```text
%LocalAppData%\Temp\i-savelev\app.log
```

### 2. Простое окно отладки

Статический класс `DebugWindow` показывает форму `Debugger` с одной таблицей. Туда можно добавлять произвольные строки и быстро смотреть промежуточные данные прямо во время работы плагина.

Это полезно, когда нужно быстро проверить:

- состав коллекции;
- промежуточные значения;
- результат обхода элементов;
- данные перед экспортом или обработкой.

## Основные методы

### Logger

- `Logger.SetLogPath(string logFile)` - задать свой путь к лог-файлу;
- `Logger.GetLogPath()` - получить текущий путь;
- `Logger.SetLogLevel(Logger.LogLevel level)` - задать минимальный уровень логирования;
- `Logger.Clear()` - очистить лог;
- `Logger.Init(...)` - записать стартовую информацию об окружении;
- `Logger.Debug(...)`, `Info(...)`, `Warning(...)`, `Error(...)`, `Critical(...)` - записать сообщение;
- `Logger.Exception(Exception ex, string message = "", LogLevel level = LogLevel.Error)` - записать исключение.

### DebugWindow

- `DebugWindow.AddRow(string str)` - добавить строку в окно;
- `DebugWindow.Show(string windowName = null)` - показать окно.

## Как использовать

### Вариант 1. Обычный лог в файл

```csharp
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace MyPlugin
{
    [Transaction(TransactionMode.Manual)]
    public class MyCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Logger.SetLogPath(
                @"C:\Temp\MyPlugin\revit.log");

            Logger.SetLogLevel(Logger.LogLevel.Debug);
            Logger.Init(
                hostName: "Autodesk Revit",
                hostVersionNumber: commandData.Application.Application.VersionNumber,
                hostBuild: commandData.Application.Application.VersionBuild,
                hasActiveDocument: commandData.Application.ActiveUIDocument != null);

            try
            {
                Logger.Info("Команда запущена");

                var doc = commandData.Application.ActiveUIDocument?.Document;
                Logger.Debug($"Имя документа: {doc?.Title ?? "нет активного документа"}");

                Logger.Info("Команда завершена успешно");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Ошибка выполнения команды");
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
```

### Вариант 2. Быстрое окно для отладки

```csharp
DebugWindow.AddRow("Старт обработки");
DebugWindow.AddRow("Найдено элементов: 125");
DebugWindow.AddRow("Текущий документ: Model_A");
DebugWindow.Show("Отладка команды");
```

Окно очищается при закрытии формы.

## Рекомендуемый сценарий для Revit-плагина

Обычно достаточно такого шаблона:

1. В начале команды вызвать `Logger.SetLogPath(...)`, если нужен свой путь.
2. Вызвать `Logger.Init(...)`, чтобы сохранить снимок окружения.
3. По ходу выполнения писать ключевые этапы через `Info(...)` и детали через `Debug(...)`.
4. Все исключения писать через `Logger.Exception(...)`.
5. При необходимости выводить промежуточные данные через `DebugWindow`.

## Что попадает в `Init(...)`

Метод пишет в лог:

- имя и версию сборки;
- имя процесса и PID;
- имя компьютера и пользователя;
- сведения об ОС;
- версию `.NET`;
- текущую культуру и таймзону;
- текущую рабочую директорию;
- командную строку процесса;
- информацию о хосте Revit, если она передана явно.

## Для чего подходит

`RevitLogger` полезен для:

- диагностики ошибок в Revit-командах;
- логирования пакетных операций;
- отладки обхода элементов модели;
- анализа проблем у пользователей по лог-файлу;
- быстрой визуальной проверки промежуточных значений без полноценного UI.

## Ограничения

- библиотека не делает ротацию логов;
- лог пишет в один файл;
- `DebugWindow` подходит только для простой отладки строками;
- библиотека не привязана к Revit API напрямую, но ориентирована на использование внутри Revit-плагинов.

## Сборка

Проект собирается как библиотека:

```text
RevitLogger.dll
```

Целевая платформа:

```text
.NET Framework 4.8
```

Основные зависимости:

- `System.Windows.Forms`
- `System.Data`
- стандартные библиотеки `.NET Framework`

