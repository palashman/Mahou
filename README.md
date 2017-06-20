[![AppVeyor build](https://ci.appveyor.com/api/projects/status/aw3qf8bevpqdidjh?svg=true)](https://ci.appveyor.com/project/BladeMight/mahou)

# Русский :ru: [English](#english-us)

![](http://i.piccy.info/i9/29fa819a849fa70f38ddfcf9c29d1c99/1480349941/6018/1094353/Mahou.jpg)

# Mahou (魔法) - Волшебный переключатель раскладок.

### Как работает
Mahou работает совершенно по-другому: в отличии от других переключателей раскладок, он переключает *не по следующей раскладке*, а по **указанным в настройках** раскладкам.<br/>
Даже выделенный текст переключается между **выбранными** раскладками, но если Вам нравилось цикличное переключение, то, начиная с версии `v1.0.2.9`, есть **Циклич. режим** (Cycle Mode).

### Для работы необходим [.Net 4.0 или выше](https://www.microsoft.com/ru-ru/download/details.aspx?id=53345). Начиная с v1.4.3.9, исправлены ошибки при работе в .Net 4.0.

### Возможности

###### Как использовать:
1. Для конвертации выделения нажмите <kbd>Scroll</kbd>, когда выделите текст.
2. Для конвертации последнего введённого слова нажмите <kbd>Pause</kbd>.
3. Для конвертации последней линии нажмите <kbd>Shift</kbd>+<kbd>Pause</kbd>.
4. Для переключения раскладки одной клавишей нажмите <kbd>CapsLock</kbd>.
4. Начиная с v1.0.4.4, при конвертации выделения текст, не распознанный ни в одной из выбранных в настройках раскладках (пример: ♥), просто переписывается.

###### Возможности по версиям:

**v2.3.0.7~v2.3.1.3**<br/>

- :gem: Добавлена новая функция - Единая раскладка для всех программ.
- :gem: Добавлена новая возможность - Всегда показывать подсказку языка возле мыши.
- :gem: Добавлена новая функция - Флаги в подсказках языка.
- :gem: Добавлено мини исправление для функции перезапуска HOOK'ов.
- :bug: Исправлена проблема в с двойными горячими клавишами состаящих только из модификаторов.

**v2.3.0.6**<br/>

- :gem: Исправлена серьёзная ошибка функции Конвертации нескольких слов из версии 2.3.0.5.

**v2.3.0.5**<br/>
- :bug: Исправлена серьёзная несовместимость подсказки языка лампочкой Scroll-Lock и функции Конвертации нескольких слов настроенной на Scroll-Lock.
- :bug: Исправлено неожиданное отключение двойных горячих клавиш в 2.3.0.0.
- :bug: Испралено невозможность запустить Mahou из-за неверных настроек шрифтов(или их отсутствия в системе) и цветов в Mahou.ini.
- :memo: Исправлены некоторые опечатки в русском языке интерфейса.
- :bug: Исправлена очистка введенных слов при Ctrl+Любая клавиша, теперь Mahou не очищает слова если *Любая клавиша* - модификатор.

**v2.3.0.0**<br/>
- :gem: Добавлена функция перезапуска HOOK'ов.
- :gem: Переписан способ регистрации горячих клавиш.
- :gem: Добавлена поддержка AltGr для функции Переключать раскладки по клавишам.
- :gem: Обновлена функция конвертации нескольких слов для поддержки нового способа регистрации горячих
- [...](https://github.com/BladeMight/Mahou/releases/v2.3.0.0)

**v2.1.2.6**<br/>
- :gem: Добавлена функция постоянной раскладки для процессов.

**v2.1.2.0~v2.1.2.3**<br/>
- :gem: Добавлена поддержка getconkbl.dll, который позволяет получить правильную раскладку в консольных приложениях. ТОЛЬКО x86 версии обоих(Mahou и getconkbl.dll) поддерживаются. (вы можете взять dll [отсюда]( https://github.com/Elfy/getconkbl), поставьте ее в папку где находится Mahou.exe, возможно потребуется перезапуск Mahou)
- :bug: Исправлены проблемы связанные с восстановлением буфера.
- :gem: Добавлена Shift+CapsLock к переключеать раскладки по клавишам.
- :speech_balloon: Обновлен способ показа подсказок, :rocket: теперь быстрее показывается, и таймаут исчезания - 20 сек.
- :gem: Обновлен способ локализации.

**v2.1.1.2~v2.1.1.6**<br/>
- Обновлена wiki.
- Добавлена функция **Считать раскладку для всего слова в КВ** которая **прекрасно** конвертирует выделенный текст в котором есть слова с символами.
- Добавлено правило — не переключать раскладку в функции **Переключать раскладки по клавишам** если до отпуска клавиши была надата кнопка мыши.
- Исправлено включение CapsLock'а при **выключенном таймере отключателе CapsLock** по Ctrl+CapsLock.
- Исправлено что символы и цифры с Numpad(**Я не про Alt+Numpad**) не ловились в текущее слово.

**v2.1.1.1**<br/>
- Добавлена возможность изменить текст для подсказок языка.
- Исправлено восстановление текста который был в буфере обмена перед конвертацией выделенного в некоторых приложениях(MS Office 2016 и т.д.).
- Исправлен вид для остальных раскладок при включенной функции **Использовать разный вид для раскладок**.
- Обновлено отладочная информация, теперь включает тэги &lt;details> и &lt;summary> для создания спойлеров для чтобы уменьшить занимаемое место в комментариях на GitHub. Данные прокси из Mahou.ini теперь не включаются в отладочную информацию.
- Исправлено сохранение данных прокси.
- Добавелно скрытие вида пароля прокси и шифрование пароля в файле настроек.

**v2.1.0.4**<br/>
- Добавлена поддержка [MCDS](https://github.com/BladeMight/MahouCaretDisplayServer), который добавляет возможность отображения подсказки языка возле каретки в Sublime Text 3.
- Добавлена функция прогамм-исключений.

**v2.0.0.3**<br/>
- Улучшено циклическое переключение раскладок. Теперь поддерживает приложения вроде WordPad и Skype(форма ввода сообщения).

**v2.0.0.0**<br/>
- Обновлен интерфейс.
- Сильно улучшены функции использующие таймеры.
- Добавлена очистка памяти, теперь Mahou потребляет ~5МБ ОЗУ.
- Добавлены функции конверсии регистра текста.
- Много исправлений.

**v1.5.2.0**<br/>
- Добавлено фунция отображения флагов стран в трее.

**v1.5.0.0**<br/>
- Добавлена функция отображения подсказки текущего языка рядом с позицией каретки (текстового курсора).
- Добавлена возможность выбора разных цветов/шрифтов для разных раскладок в подсказках языка.

**v1.4.4.1**<br/>
- Добавлена функция *Отладочная инф.* (Debug Info), которая копирует отладочную информацию в буфер обмена.

**v1.4.3.2**<br/>
- Добавлена функция логирования (журналирования) для поиска ошибок.

**v1.4.3.0**<br/>
- Переписана функция конверт линии для поддержки новой функции Конверт нескольких последних слов.

**v1.4.2.1**<br/>
- Добавлена возможность отключения проверки обновления при запуске (полезно для пользователей Chocolatey, т.к. там есть `cup all -y`).

**v1.4.1.7**<br/>
- Добавлена возможность подсветки Scroll Lock при активном языке 1.

**v1.4.1.6**<br/>
- Добавлена возможность ввода символов Alt+Numpad с их последующей конвертацией.

**v1.4.0.0**<br/>
- Добавлена возможность установки сниппетов (автозамена слов на другие слова/куски текста).

**v1.3.1.0**<br/>
- Добавлена возможность установки прозрачного фона для подсказки текущего языка.

**v1.3.0.0**<br/>
- Добавлена возможность двойных горячих клавиш (2x<kbd>Shift</kbd>, и т.д.).

**v1.1.5.6**<br/>
- Добавлена маленькая подсказка текущего языка при наведении мыши на текст.

**v1.1.2.0**<br/>
- Добавлен движок языков и русский язык.

**v1.1.1.0**<br/>
- Добавлена возможность `Расширенная настройка CTRL'ов`.

**v1.1.0.0**<br/>
- Добавлена возможность `Авто-Восстановления` ТЕКСТА для Конверт выделения.

**v1.0.9.6**<br/>
- Возможность `Use Alt+Shift in CM` улучшена в `Эму` (которая более настраиваемая).

**v1.0.8.7**<br/>
- Добавлена возможность `"" " ←` (Съесть один Space).

**v1.0.7.9**<br/>
- Новый, более совместимый способ сохранять/загружать настройки.

**v1.0.4.7**<br/>
- Возможность `Обновление` улучшена в `Авто-обновление` (Auto Update).

**v1.0.4.2**<br/>
- Добавлена возможность `Обновление` (Update).

**v1.0.4.0**<br/>
- Добавлена возможность `Конверт линии` (Convert Line).

**v1.0.2.9**<br/>
- Добавлена возможность `Циклич. режим` (Cycle Mode) которая переключает раскладки циклично. Применимо к `CapsSwitch`.

**v1.0.0.7**<br/>
- Добавлена возможность `CapsSwitch` - возможность переключать раскладки нажатием CapsLock.

**v1.0.0.4**<br/>
- Добавлена возможность изменять горячие клавиши для `Конверт слова` (Convert Last) & `Конверт выделения` (Convert Selection).

### Горячие клавиши
- <kbd>Pause</kbd> - Конверт последнего слова.
- <kbd>Shift</kbd>+<kbd>Pause</kbd> - Конверт линии.
- <kbd>Scroll</kbd> - Конверт выделения.
- <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>Alt</kbd>+<kbd>Insert</kbd> - Показать/скрыть главное окно.
- <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>Alt</kbd>+<kbd>F12</kbd> - Завершить Mahou.
- [Подробнее...](https://github.com/BladeMight/Mahou/wiki/%D0%A1%D0%BF%D0%B8%D1%81%D0%BE%D0%BA-%D1%84%D1%83%D0%BD%D0%BA%D1%86%D0%B8%D0%B9#%D0%93%D0%BE%D1%80%D1%8F%D1%87%D0%B8%D0%B5-%D0%BA%D0%BB%D0%B0%D0%B2%D0%B8%D1%88%D0%B8)

### [Скачать или посмотреть заметки о выпусках.](https://github.com/BladeMight/Mahou/releases)

### Используете Chocolatey? `cinst Mahou` для установки.

### Wiki
Ознакомьтесь с [Mahou Wiki](https://github.com/BladeMight/Mahou/wiki).

### Лицензия
Mahou находится под GPL v2+.

### Связаться со мной
Если Вы нашли ошибку напишите её [здесь](https://github.com/BladeMight/Mahou/issues)
или свяжитесь со мной через [email](mailto:BladeMight@gmail.com) (можете спрашивать о чем угодно)

### Поддержать проект материально

Если Вы считаете что хотите помочь материально, буду очень благодарен :)

Кошельки:

- Яндекс.Деньги: 410015057363201
- Webmoney WMZ: Z407834572196

# English :us:

![](http://i.piccy.info/i9/29fa819a849fa70f38ddfcf9c29d1c99/1480349941/6018/1094353/Mahou.jpg)

# Mahou (魔法) - The magic Layout Switcher

### How it works
Mahou works completely different from other Layout Swtichers, it switches *not by next layout*, but by **specified in settings** layouts. <br/>
Even selected text switches just between **selected** layouts, though if you liked cycling through, starting from `v1.0.2.9` there is **Cycle Mode**.

### Mahou requires [.Net 4.0 or greater](https://www.microsoft.com/en-us/download/details.aspx?id=53345) to work properly. Beginning from v1.4.3.9 error when running on .Net 4.0 were fixed.

### Features

###### How to use:
1. To convert selection hit <kbd>Scroll</kbd> when select text.
2. To convert input hit <kbd>Pause</kbd> when typing.
3. To convert line hit <kbd>Shift</kbd>+<kbd>Pause</kbd>.
4. To change layout by one key press <kbd>CapsLock</kbd>.
4. Starting from v1.0.4.4 in Convert selection unrecognized text by all selected layout in settings (example: ♥) just rewrites.

###### By version features:

**v2.3.0.7~v2.3.1.3**<br/>

- :gem: Added new feature - One(global) layout.
- :gem: Added new feature - Always show mouse tooltip.
- :speech_balloon: Added new tooltips.
- :gem: Added new feature, flags in language tooltips.
🐛 Fixed double hotkey feature for only-modifiers hotkeys (from v2.3.0.5).

**v2.3.0.6**<br/>

- :gem: Fixed serious bug in Convert Multiple Words function from v2.3.0.5.

**v2.3.0.5**<br/>

- :bug: Fixed double hotkey ability was inacessible until restart in v2.3.0.0.
- :gem: Fixed serious uncompatibility between Scroll-Tip and Convert Multiple words as Scroll hotkey.
- :bug: Fixed Mahou wasn't start up if fonts or colors are wrongly configured in Mahou.ini.
- :gem: Fixed Mahou was clearing catched words on Ctrl+Any key(even modifier) modifiers were excluded.
- :memo: Fixed some typos in Russian UI translation. подсказки языка лампочкой Scroll-Lock и функции Конвертации нескольких слов настроенной на Scroll-Lock.

**v2.3.0.0**<br/>
- :gem: Added Restart Hooks on each Mahou hotkey action end.
- :gem: Hotkeys overhaul migrated to Windows-style(RegisterHotkey()) which is a way more stable.
- :gem: Added AltGr support for switch layout with one key.
- :gem: Updated Convert Multiple words style to new Hotkeys style.
- [...](https://github.com/BladeMight/Mahou/releases/v2.3.0.0)

**v2.1.2.6**<br/>
- :gem: Added Persistent layout function for processes.

**v2.1.2.0~v2.1.2.3**<br/>
- :gem: Added support for getconkbl.dll, which adds support for console apps right layout recognition. ONLY x86 version of both(Mahou and getconkbl.dll) are supported. (you can get dll from [here]( https://github.com/Elfy/getconkbl), put it in directory where Mahou.exe is)
- :bug: Fixed some clipboard restore issues.
- :gem: Added Shift+CapsLock to by key switch.
- :speech_balloon: Updated tooltip appear style, :rocket: now faster (re)show, and timeout to hide to 20 sec.
- :gem: Updated translation to dictionary style.

**v2.1.1.2~v2.1.1.6**<br/>
- Updated wiki.
- Added fucntion **Use layout for whole word in CS** which **perfectly** converts selected text in which are words with symbols.
- Added rule — not switch layout in **Change to specific layout by key** if before key release was clicked mouse button.
- Fixed Ctrl+CapsLock enabling when **CapsLock disabler timer** is off.
- Fixed Numpad numbers and symbols(i'm talking not about alt+numpad) was not catched in last word/words.

**v2.1.1.1**<br/>
- Added feature to change language tooltip text.
- Fixed clipboard text restore before converting selection in some apps(MS Office 2016 etc.)..
- Fixed appearence for non-main-layouts(two in settings) when *Use different appearence for layouts* enabled.
- Updated debug info, now it includes tags &lt;details> and &lt;summary> to create spoilers which will consume less space in comments on GitHub. Proxy settings from Mahou.ini now won't be included to debug info.
- Fixed proxy settings saving.
- Added proxy password hide in view and password encryption in the settings file.

**v2.1.0.4**<br/>
- Added support for [MCDS](https://github.com/BladeMight/MahouCaretDisplayServer), which adds ability to display caret language tooltip in Sublime Text 3.
- Added excluded programs function.

**v2.0.0.3**<br/>
- Improved layout switching by cycle. Now it supports apps like WordPad and Skype(message entry form).

**v2.0.0.0**<br/>
- Updated interface.
- Greatly increased speed of functions that using timers.
- Added memory clearing, now Mahou uses ~5МБ RAM.
- Added functions to convert text case.

**v1.5.2.0**<br/>
- ِAdded feature to display country flags in tray.

**v1.5.0.0**<br/>
- Added feature to display language tooltip around caret (carriage) position.
- Added ability to select different color for different layouts in language tooltips.

**v1.4.4.1**<br/>
- Added new feature *Debug Info*, copies useful debug info for posting issues.

**v1.4.3.2**<br/>
- Added feature logging, for debugging.

**v1.4.3.0**<br/>
- Rewrited convert line feature to support new feature, Convert Multiple last words.

**v1.4.2.1**<br/>
- Added feature to disable check for update at startup (useful for Chocolatey users, choco has `cup all -y`).

**v1.4.1.7**<br/>
- Added feature to highlight Scroll Lock when language 1 is active.

**v1.4.1.6**<br/>
- Added feature to catch Alt+Numpad symbols to use them in convert last/line.

**v1.4.0.0**<br/>
- Added feature "Snippets" (expand words to other words/text fragments).

**v1.3.1.0**<br/>
- Added ability to set transparent background for language tooltip.

**v1.3.0.0**<br/>
- Added double hotkey ability (2x<kbd>Shift</kbd>, etc.).

**v1.1.5.6**<br/>
- Added small tip which displays current layout, when hovering text with mouse.

**v1.1.2.0**<br/>
- Added language engine and Russian language.

**v1.1.1.0**<br/>
- Added feature "Extended CTRLs config".

**v1.1.0.0**<br/>
- Added TEXT auto-backup feature for convert selection.

**v1.0.9.6**<br/>
- Feature "Use Alt+Shift in CM" upgraded to "Emu" (Which is more customizable).

**v1.0.8.7**<br/>
- New feature """ " ←" (Eat one space).

**v1.0.7.9**<br/>
- New method to save/load configuration which is more compatible.

**v1.0.4.7**<br/>
- Improved `Update` feature into `Auto Update`.

**v1.0.4.2**<br/>
- Added `Update` feature.

**v1.0.4.0**<br/>
- Added `Convert Line` feature.

**v1.0.2.9**<br/>
- It is possible to switch to cycle mode, that switches to next layout. This also applies to `CapsSwitch`.

**v1.0.0.7**<br/>
- Added feature `CapsSwitch` - possible to toggle layouts by CapsLock.

**v1.0.0.4**<br/>
- Added ability to change hotkeys for `Convert Last` & `Convert Selection`.

### Hotkeys
- <kbd>Pause</kbd> - Convert last input.
- <kbd>Shift</kbd>+<kbd>Pause</kbd> - Convert last inputted line.
- <kbd>Scroll</kbd> - Convert selected text.
- <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>Alt</kbd>+<kbd>Insert</kbd> - To toggle configs windows visibility.
- <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>Alt</kbd>+<kbd>F12</kbd> - To exit Mahou.
- [More...](https://github.com/BladeMight/Mahou/wiki/Functions-list#hotkeys)

### [Download or view release notes.](https://github.com/BladeMight/Mahou/releases)

### Using Chocolatey? type `cinst Mahou` to install.

### Wiki
Check out [Mahou Wiki](https://github.com/BladeMight/Mahou/wiki).

### License
Mahou is under GPL v2+

### Contact
If you found an issue write [here](https://github.com/BladeMight/Mahou/issues)
or contact me though [email](mailto:BladeMight@gmail.com) (You can ask anything)

### Donate

If you think that you want to help financially, I will be very grateful :)

Wallets:

- Yandex.Money: 410015057363201
- Webmoney WMZ: Z407834572196
